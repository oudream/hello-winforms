using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using HelloWinForms.Utilities;
using System.Threading;
using System.Reflection;
using Serilog.Core;
using CxWorkStation.Utilities;
using System.Diagnostics;
using YamlDotNet.Core;
using System.Globalization;
using CommonInterfaces;

namespace HelloWinForms
{
    public static class DaemonHelper
    {
        private static List<string> _messageUpdateList = new List<string>();
        private static volatile bool _isRunning = false;
        private static AutoResetEvent _updateSignal = new AutoResetEvent(false);
        private static readonly object _lockUpdateList = new object();
        private static Dictionary<string, Process> _processes = new Dictionary<string, Process>();

        private static List<DaemonAppConfig> _apps = new List<DaemonAppConfig>();

        public static void Init()
        {
            var apps = LoadDaemons();
            if (apps != null)
            {
                _apps = apps;
            }
        }

        public static void Run()
        {
            if (_isRunning) return;
            _isRunning = true;

            long dtMonitor = 0;

            new Thread(() =>
            {
                try
                {
                    while (_isRunning)
                    {
                        // 等待一小段时间或直到收到更新信号
                        _updateSignal.WaitOne(TimeSpan.FromMilliseconds(100));

                        // 图像存储队列
                        List<string> messageEntries = null;

                        lock (_lockUpdateList)
                        {
                            if (_messageUpdateList.Count > 0)
                            {
                                messageEntries = new List<string>(_messageUpdateList);
                                _messageUpdateList.Clear();
                            }
                        }

                        // 处理图像存储队列处理
                        if (messageEntries != null && messageEntries.Count > 0)
                        {
                            DealMessages(messageEntries);// 处理图像
                            messageEntries.Clear();
                        }


                        var dtNow = TimeHelper.GetNow();
                        if (dtNow - dtMonitor > 3000)
                        {
                            MonitorAndDaemonApps();
                            dtMonitor = dtNow;
                        }

                    }
                }
                catch (Exception ex)
                {
                    _isRunning = false;
                    // 记录异常
                    LogHelper.Error($"Exception in DaemonHelper thread, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                    // 再次抛出
                    throw;
                }

                _isRunning = false;
            })
            { IsBackground = true }.Start();
        }

        private static void MonitorAndDaemonApps()
        {
            // 先获取系统中所有运行的进程列表
            var runningProcesses = Process.GetProcesses();

            foreach (var app in _apps)
            {
                bool isRunning = false;

                // 检查运行中的进程列表是否包含当前应用
                var fn = Path.GetFileNameWithoutExtension(app.FileName);
                foreach (var process in runningProcesses)
                {
                    if (process.ProcessName.Equals(fn, StringComparison.OrdinalIgnoreCase))
                    {
                        isRunning = true;
                        app.StartTime = TimeHelper.GetMs(process.StartTime);

                        // 进程正在运行，打印进程信息
                        LogHelper.Debug($"看守服务 进程 {fn} 正在运行，内存占用: {process.WorkingSet64 / 1024} KB，线程数: {process.Threads.Count}，句柄数: {process.HandleCount}");
                        break;
                    }
                }

                // 如果进程未运行，则启动它
                if (!isRunning)
                {
                    app.StartTime = 0;
                    app.StartCompleteTime = 0;
                    LogHelper.Debug($"看守服务 未检测到进程 {fn}，正在启动...");
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = app.FileName,
                        Arguments = app.Arguments,
                        WorkingDirectory = app.WorkingDirectory,
                        CreateNoWindow = app.CreateNoWindow,
                        UseShellExecute = false
                    };

                    try
                    {
                        var process = Process.Start(startInfo);
                        app.StartTime = TimeHelper.GetNow();
                        if (!string.IsNullOrEmpty(app.AppLogFileName))
                        {
                            // 检测 infer.log 最后一行要出现（2024-10-15 08:25:53 [INF] Server listening on port 12345...），而且时间要大于启动时间，才算运行成功
                            // 每 200 毫秒检查一次，最多检查 10 秒，确保启动成功  
                            bool isStarted = CheckAppStatusWithTimeout(app, process.StartTime, 10000, 200);
                            if (isStarted)
                            {
                                app.StartCompleteTime = TimeHelper.GetNow();
                                _processes[fn] = process;
                            }
                            else
                            {
                                LogHelper.Debug($"看守服务 启动进程 {fn} 失败");
                                process.Kill();
                            }
                        }
                        else
                        {
                            _processes[fn] = process;
                        }
                        LogHelper.Debug($"看守服务 已启动进程 {fn}");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"看守服务 启动进程 {fn} 失败: {ex.Message}");
                    }
                }
            }
        }

        // 每200毫秒检查一次是否启动成功，最多检查10秒
        private static bool CheckAppStatusWithTimeout(DaemonAppConfig app, DateTime startTime, int timeoutMs, int intervalMs)
        {
            DateTime startCheckTime = DateTime.Now;  // 记录检查开始的时间

            while ((DateTime.Now - startCheckTime).TotalMilliseconds < timeoutMs)
            {
                if (IsAppRunningSuccessfully(startTime, app))
                {
                    return true;
                }
                Thread.Sleep(intervalMs);
            }
            return false;
        }

        // 检查 infer.log 是否包含成功启动的信息，且时间大于进程启动时间
        private static bool IsAppRunningSuccessfully(DateTime startTime, DaemonAppConfig app)
        {
            try
            {
                string logFilePath = Path.Combine(app.WorkingDirectory, app.AppLogFileName);
                if (File.Exists(logFilePath))
                {
                    string lastLogLine = ReadLastLineWithContent(logFilePath) ;

                    if (!string.IsNullOrEmpty(lastLogLine))
                    {
                        // 检查日志是否包含启动成功的消息
                        string successMessage = "[INF] Server listening on port";
                        if (lastLogLine.Contains(successMessage))
                        {
                            // 从日志行中提取时间
                            string timestamp = lastLogLine.Substring(0, 19); // 假设时间戳是前19个字符
                            DateTime logTime = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                            // 检查日志时间是否大于进程启动时间
                            if (logTime > startTime)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查日志文件时出错: {ex.Message}");
            }

            return false;
        }

        public static string ReadLastLineWithContent(string filePath)
        {
            const int bufferSize = 1024; // 每次读取1KB的内容
            byte[] buffer = new byte[bufferSize];

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length == 0)
                {
                    return string.Empty; // 文件为空
                }

                long position = fs.Length;
                StringBuilder sb = new StringBuilder();
                bool foundLine = false;

                while (position > 0)
                {
                    int bytesToRead = (int)Math.Min(bufferSize, position); // 如果文件尾部不够1KB，则读取剩余部分
                    position -= bytesToRead;
                    fs.Seek(position, SeekOrigin.Begin);
                    fs.Read(buffer, 0, bytesToRead);

                    string chunk = Encoding.Default.GetString(buffer, 0, bytesToRead);
                    sb.Insert(0, chunk);

                    // 分割成行
                    string[] lines = sb.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    // 从最后一行开始查找有内容的行
                    for (int i = lines.Length - 1; i >= 0; i--)
                    {
                        if (!string.IsNullOrWhiteSpace(lines[i]))
                        {
                            foundLine = true;
                            return lines[i];
                        }
                    }

                    if (foundLine)
                    {
                        break;
                    }

                    sb.Clear(); // 清空 StringBuilder，以便继续从文件中读取更多数据
                }
            }

            return string.Empty; // 如果没有找到任何内容行，返回空字符串
        }

        // 判断是否所有的应用都启动成功
        public static bool AreAllAppsStarted()
        {
            foreach (var app in _apps)
            {
                if (!string.IsNullOrEmpty(app.AppLogFileName))
                {
                    if (app.StartCompleteTime <= 0)
                    {
                        return false;
                    }
                }
                else if (app.StartTime <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        // 一次性获取所有正在运行的进程，并停止与配置中匹配的守护进程
        private static void StopAllApps()
        {
            // 获取当前系统中所有运行的进程
            var runningProcesses = Process.GetProcesses();

            foreach (var app in _apps)
            {
                // 查找与配置中的应用程序匹配的进程
                foreach (var process in runningProcesses)
                {
                    if (process.ProcessName.Equals(Path.GetFileNameWithoutExtension(app.FileName), StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            process.Kill();
                            LogHelper.Debug($"看守服务 已停止进程 {process.ProcessName}");
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Debug($"看守服务 停止进程 {process.ProcessName} 时出错: {ex.Message}");
                        }
                    }
                }
            }
        }

        // isForce: 如果服务停止也退出所有app，就退出所有app
        public static void Stop(bool isForce = false)
        {
            _isRunning = false;
            // 通知线程
            _updateSignal.Set();
            // 如果服务停止也退出所有app，就退出所有app
            if (isForce)
            {
                StopAllApps();
            }
        }

        // 图像保存
        private static void DealMessages(List<string> entries)
        {
            // 按队列来保存图像
            foreach (var entry in entries)
            {
                DealMessage(entry);
            }
        }

        private static bool DealMessage(string message)
        {
            return true;
        }

        public static void PushMessage(string message)
        {
            // 压入队列
            lock (_lockUpdateList)
            {
                _messageUpdateList.Add(message);
            }
            // 通知线程
            _updateSignal.Set();
        }

        private static List<DaemonAppConfig> LoadDaemons()
        {
            string configFilePath = "daemon_config.yaml";  // 配置文件路径

            try
            {
                // 读取配置文件内容
                string yamlContent = File.ReadAllText(configFilePath);

                // 创建序列化器，用于反序列化 YAML 文件
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();

                // 反序列化 YAML 文件为守护进程配置列表
                var result = deserializer.Deserialize<Dictionary<string, List<DaemonAppConfig>>>(yamlContent);

                List<DaemonAppConfig> apps = result["apps"];
                // 打印每个守护进程的配置信息
                foreach (var app in apps)
                {
                    LogHelper.Debug($"看守服务 加载APP - FileName: {app.FileName}, Arguments: {app.Arguments}, WorkingDirectory: {app.WorkingDirectory}, CreateNoWindow: {app.CreateNoWindow}");
                }

                return apps;
            }
            catch (Exception e)
            {
                // 加载配置文件失败时的错误处理
                LogHelper.Error($"看守服务 加载APP配置列表失败: {e.Message}");
            }
            return null;
        }

        // 保存样本配置
        public static void SaveSampleDaemons()
        {
            // Sample configurations
            var daemon1 = new DaemonAppConfig
            {
                FileName = "D:\\cyg\\FastDeployV31\\infer_demo.exe",
                Arguments = "D:\\cyg\\FastDeployV31\\model",
                WorkingDirectory = "D:\\cyg\\FastDeployV31",
                CreateNoWindow = true,
                AppLogFileName = "D:\\cyg\\FastDeployV31\\infer.log"
            };

            var daemon2 = new DaemonAppConfig
            {
                FileName = "D:\\cyg\\VCDataAnalysis\\VCDataAnalysis.exe",
                Arguments = "",
                WorkingDirectory = "D:\\cyg\\VCDataAnalysis",
                CreateNoWindow = false
            };

            var daemonConfig = new
            {
                Apps = new List<DaemonAppConfig> { daemon1, daemon2 }
            };

            // Serialize to YAML
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted)
                .WithIndentedSequences() // 确保列表项前有正确的缩进
                .Build();
            var yaml = serializer.Serialize(daemonConfig);

            // Save YAML to file
            File.WriteAllText("daemon_config.yaml", yaml);

            LogHelper.Error($"看守服务 保存样本配置 daemon_config.yaml");
        }
    }

    public class DaemonAppConfig
    {
        public string FileName { get; set; }           // 守护进程的可执行文件名
        public string Arguments { get; set; }          // 守护进程的启动参数
        public string WorkingDirectory { get; set; }   // 守护进程的工作目录
        public bool CreateNoWindow { get; set; }       // 是否隐藏窗口
        public string AppLogFileName { get; set; }            // 进程的日志文件路径
        [YamlIgnore]
        public long StartTime { get; set; }        // 启动时间
        [YamlIgnore]
        public long StartCompleteTime { get; set; }        // 启动完成时间
    }

}
