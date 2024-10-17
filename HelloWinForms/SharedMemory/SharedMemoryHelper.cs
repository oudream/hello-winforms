using CommonInterfaces;
using CxWorkStation.Utilities;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CxAAC.Utilities
{
    public class SharedMemoryHelper
    {
        private const int RequestSize = 1024 * 1024;   // 1MB for request
        private const int ResponseSize = 1024 * 1024;  // 1MB for response
        private const int RequestImageSize = 40 * 1024 * 1024; // 40MB for images
        private const int ResponseImageSize = RequestImageSize; // 40MB for images
        private const int TotalMemorySize = (RequestSize + ResponseSize + RequestImageSize + ResponseImageSize);

        private const string ImageSizeKey = "ImageSize";

        private MemoryMappedFile memoryMappedFile;
        private MemoryMappedViewAccessor accessor;

        private List<SharedMemoryImageEntry> _updateList = new List<SharedMemoryImageEntry>();
        private volatile bool _isRunning = false;
        private AutoResetEvent _updateSignal = new AutoResetEvent(false);
        private readonly object _lockUpdateList = new object();

        // 检测结果推送
        // 线号、检测号、结果数据
        public event Action<uint, uint, byte[]> DetectionResultReleased;

        public SharedMemoryHelper(string sharedMemoryName)
        {
            // 创建或打开共享内存
            memoryMappedFile = MemoryMappedFile.CreateOrOpen(sharedMemoryName, TotalMemorySize);
            accessor = memoryMappedFile.CreateViewAccessor(0, TotalMemorySize);
        }

        public void Run()
        {
            if (_isRunning) return;
            _isRunning = true;
            new Thread(() =>
            {
                try
                {
                    while (_isRunning)
                    {
                        // 等待一小段时间或直到收到更新信号
                        //_updateSignal.WaitOne(); // 等待更新信号
                        _updateSignal.WaitOne(TimeSpan.FromMilliseconds(30));

                        // 图像存储队列
                        List<SharedMemoryImageEntry> sharedMemoryImageEntries = null;

                        lock (_lockUpdateList)
                        {
                            if (_updateList.Count > 0)
                            {
                                sharedMemoryImageEntries = new List<SharedMemoryImageEntry>(_updateList);
                                _updateList.Clear();
                            }
                        }

                        // 处理图像存储队列处理
                        if (sharedMemoryImageEntries != null && sharedMemoryImageEntries.Count > 0)
                        {
                            DealSharedMemoryImagesRequest(sharedMemoryImageEntries);// 处理图像
                            sharedMemoryImageEntries.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _isRunning = false;
                    // 记录异常
                    Console.WriteLine($"Exception in StorageImageHelper thread, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                    // 再次抛出
                    throw;
                }

                _isRunning = false;
            })
            { IsBackground = true }.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            // 通知线程
            _updateSignal.Set();
        }

        // 图像保存
        private void DealSharedMemoryImagesRequest(List<SharedMemoryImageEntry> entries)
        {
            // 按队列来保存图像
            foreach (var entry in entries)
            {
                DealSharedMemoryImageRequest(entry);
                entry.Image.Dispose();
            }
        }

        private void DealSharedMemoryImageRequest(SharedMemoryImageEntry entry)
        {
            try
            {
                if (HalconHelper.GetImageInfo(entry.Image, out int width, out int height, out int channels, out int bitDepth, out IntPtr dataPtr))
                {
                    if (width <= 0 || height <= 0 || channels != 1 || bitDepth != 8)
                    {
                        Console.WriteLine($"AI推理Box 初始化时加载图像失败，图像大小不符合要求，width:{width} height:{height} channels:{channels} bitDepth:{bitDepth}");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"AI推理Box 初始化时加载图像失败，获图像信息失败");
                    return;
                }
                // 生成请求信息并发送
                var imageSize = width * height * channels;
                var dtNow = TimeHelper.GetNow();
                string request = GenerateRequest("DetectFile", "imageName1", imageSize, dtNow, 0.5f, width, height, 0);

                // 向共享内存中写入请求信息
                byte[] requestData = Encoding.ASCII.GetBytes(request);
                accessor.WriteArray(0, requestData, 0, requestData.Length);
                Console.WriteLine("请求已发送到共享内存中");

                var (success, responseMap, imageData) = ScanForResponses();
                if (success)
                {
                    // 推送检测结果
                    DetectionResultReleased?.Invoke(entry.LineNumber, entry.BatchNumber, imageData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI推理Box Exception : {ex.Message}");
            }
        }

        public void PushImage2SharedMemory(uint lineNumber, uint batchNumber, HObject image)
        {
            // 压入队列
            lock (_lockUpdateList)
            {
                _updateList.Add(new SharedMemoryImageEntry { LineNumber = lineNumber, BatchNumber = batchNumber, Image = image });
            }
            // 通知线程
            _updateSignal.Set();
        }

        // 生成请求信息的函数
        public string GenerateRequest(string command, string imageName, int imageSize, long timestamp, float score, int height, int width, int matTypeInt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Command={command}");
            sb.AppendLine($"ImageName={imageName}");
            sb.AppendLine($"{ImageSizeKey}={imageSize}");
            sb.AppendLine($"Timestamp={timestamp}");
            sb.AppendLine($"Score={score}");
            sb.AppendLine($"Height={height}");
            sb.AppendLine($"Width={width}");
            sb.AppendLine($"MatTypeInt={matTypeInt}");
            return sb.ToString();
        }

        // 解析反馈信息的函数
        public Dictionary<string, string> ParseResponse(string response)
        {
            Dictionary<string, string> responseMap = new Dictionary<string, string>();
            string[] lines = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    responseMap[parts[0]] = parts[1];
                }
            }

            return responseMap;
        }

        // 扫描反馈结果
        private (bool, Dictionary<string, string>, byte[]) ScanForResponses()
        {
            var dtNow = TimeHelper.GetNow();
            while (true)
            {
                byte[] responseData = new byte[ResponseSize];
                accessor.ReadArray(RequestSize, responseData, 0, responseData.Length);
                string response = Encoding.ASCII.GetString(responseData).TrimEnd('\0');

                if (!string.IsNullOrWhiteSpace(response))
                {
                    Dictionary<string, string> responseMap = ParseResponse(response);
                    Console.WriteLine("收到反馈信息:");
                    foreach (var pair in responseMap)
                    {
                        Console.WriteLine($"{pair.Key} = {pair.Value}");
                    }

                    var imageSize = int.Parse(responseMap["ImageSize"]);
                    if (imageSize <= 0 || imageSize > RequestImageSize)
                    {
                        throw new Exception("图像大小不符合要求");
                    }
                    // 检查是否收到图像（假设图像存储在40MB的块中）
                    byte[] imageData = new byte[imageSize];
                    accessor.ReadArray(RequestSize + ResponseSize + RequestImageSize, imageData, 0, imageData.Length);
                    if (imageData.Length > 0)
                    {
                        Console.WriteLine("图像已收到（模拟图像数据的读取）");
                        // 处理图像数据 (例如保存到文件或显示)
                    }

                    // 清空反馈信息以避免重复读取
                    byte[] emptyData = new byte[ResponseSize];
                    accessor.WriteArray(RequestSize, emptyData, 0, emptyData.Length);

                    return (true, responseMap, imageData);
                }
                else
                {
                    // 超时10秒
                    const long ResponseTimeout = 1000 * 10;
                    // 判断是否超时
                    if ((dtNow - TimeHelper.GetNow()) > ResponseTimeout)
                    {
                        return (false, null, null);
                    }
                }

                Thread.Sleep(50);  // 每秒检查一次反馈信息
            }
        }

        public void Dispose()
        {
            accessor.Dispose();
            memoryMappedFile.Dispose();
        }
    }

    // MainProcessingHelper 线程间的数据结构（接收任务源）
    // 要处理的图像信息
    public struct SharedMemoryImageEntry
    {
        // 检测号
        public uint LineNumber;
        // 检测号
        public uint BatchNumber;
        // 待处理的图像
        public HObject Image;
    }

}
