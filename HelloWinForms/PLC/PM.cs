using CommonInterfaces;
using CxWorkStation.Utilities;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HelloWinForms.PLC
{
    /// <summary>
    /// 实现 PLCProcessTask 的二进制流式存储和读取
    /// 支持写入完整记录（带记录长度前缀）以及反向扫描最新100条记录，并以 (LineNumber, BatchNumber) 为 Key 过滤，只保留最新一条。
    /// </summary>
    public class PLCProcessTaskSink : IDisposable
    {
        private FileStream _fileStream;
        private BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly object _syncRoot = new object();
        private readonly bool _forWriting;
        private long _todayEndMs;

        #region 构造函数

        /// <summary>
        /// 构造函数（写入模式）
        /// </summary>
        /// <param name="append">是否追加写入</param>
        public PLCProcessTaskSink(bool append = true)
        {
            _todayEndMs = TimeHelper.GetTodayEndMs();
            var path = ConfigHelper.LogFilePath(GetNowFileNameBasedOnUnixDay(TimeHelper.GetNow()));
            EnsureDirectoryExists(path);
            _fileStream = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
            _writer = new BinaryWriter(_fileStream, Encoding.UTF8);
            _forWriting = true;
        }

        /// <summary>
        /// 构造函数（读取模式）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="mode">文件访问模式（应使用 FileAccess.Read）</param>
        public PLCProcessTaskSink(FileAccess mode)
        {
            // 如果是跨天崩溃，那就活该了
            var path = ConfigHelper.LogFilePath(GetNowFileNameBasedOnUnixDay(TimeHelper.GetNow()));
            _fileStream = new FileStream(path, FileMode.Open, mode, FileShare.Read);
            _reader = new BinaryReader(_fileStream, Encoding.UTF8);
            _forWriting = false;
        }

        private void EnsureDirectoryExists(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        #endregion

        #region 只保留2天，交替覆盖写入文件
        // 根据 Unix 时间的天数计算文件名（0、1 交替）
        private static string GetNowFileNameBasedOnUnixDay(long ms)
        {
            long unixDays = ms / ( 86400 * 1000 ); // 获取 Unix 天数
            return (unixDays % 2 == 0) ? "pcl_process_task_0.log" : "pcl_process_task_1.log"; // 根据奇偶交替
        }
        #endregion

        #region 写入方法

        /// <summary>
        /// 写入前比较创建时间是否超出当前天的结束时间，超出则交替覆盖写入文件。
        /// 写入一个 PLCProcessTask 对象（写入时包含记录长度前缀）。
        /// 写入完成后不自动 Flush，由调用方决定何时调用 Flush()。
        /// </summary>
        /// <param name="task">PLCProcessTask 对象</param>
        public void Write(PLCProcessTask task)
        {
            if (!_forWriting)
                throw new InvalidOperationException("Sink opened in read mode");

            lock (_syncRoot)
            {
                // 检查 task.CreateTime 是否超过当前天的结束时间
                long taskTimeMs = TimeHelper.GetMs(task.CreateTime);
                if (taskTimeMs > _todayEndMs)
                {
                    // 需要切换文件
                    _fileStream.Close();
                    _writer.Close();

                    // 计算新的文件名（0、1交替）
                    string newFileName = GetNowFileNameBasedOnUnixDay(taskTimeMs);
                    string newFilePath = Path.Combine(Path.GetDirectoryName(_fileStream.Name), newFileName);

                    // 重新打开文件，覆盖写入
                    _fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    _writer = new BinaryWriter(_fileStream, Encoding.UTF8);

                    // 更新今天的结束时间
                    _todayEndMs = TimeHelper.GetTodayEndMs();
                    LogHelper.Debug($"超出当前天结束时间，切换写入文件: {newFilePath}");
                }

                // 先序列化到内存流，获取字节数组和长度
                using (var ms = new MemoryStream())
                {
                    using (var bw = new BinaryWriter(ms, Encoding.UTF8, true))
                    {
                        // 写入顺序必须与 ReadNext/DeserializeTask 中读取顺序一致
                        bw.Write(task.LineNumber);
                        bw.Write(task.BatchNumber);
                        bw.Write(task.TrayPos);
                        bw.Write(task.CreateTime.Ticks);
                        bw.Write(task.DeterminationMode);
                        bw.Write(task.CurrentProduct);
                        bw.Write(task.ProductBatchCode);
                        bw.Write((int)task.InspectionType);
                        bw.Write((int)task.DetectionReason);
                        bw.Write(task.Voltage);
                        bw.Write(task.Current);

                        // 写入 ProductsInfo 数组
                        bw.Write(task.ProductsInfo.Length);
                        foreach (var product in task.ProductsInfo)
                        {
                            bw.Write(product.Position);
                            bw.Write(product.HasProduct);
                            bw.Write(product.SN);
                            bw.Write((int)product.ResultStatus);
                            bw.Write(product.ResultStatusString);
                            bw.Write(product.CheckToken);
                            bw.Write(product.ScanFail);
                            bw.Write(product.PredictionImagePath);
                            bw.Write(product.BeJudgementUser);
                            bw.Write(product.MesUploadResult.Item1);
                            bw.Write(product.MesUploadResult.Item2);
                        }
                    }
                    byte[] recordBytes = ms.ToArray();
                    // 先写入记录长度（4字节 Int32），再写入记录内容
                    _writer.Write(recordBytes.Length);
                    _writer.Write(recordBytes);
                    _writer.Write(recordBytes.Length);
                }
                // 不自动 Flush，由调用方控制
            }
        }

        /// <summary>
        /// 由调用者主动刷新缓冲区到磁盘
        /// </summary>
        public void Flush()
        {
            if (!_forWriting)
                throw new InvalidOperationException("Sink opened in read mode");

            lock (_syncRoot)
            {
                _writer.Flush();
            }
        }

        #endregion

        #region 正向读取

        /// <summary>
        /// 顺序读取下一个 PLCProcessTask 对象（正向读取）。
        /// 注意：必须保证写入和读取的字段顺序一致。
        /// </summary>
        /// <returns>PLCProcessTask 对象</returns>
        public PLCProcessTask ReadNext()
        {
            if (_forWriting)
                throw new InvalidOperationException("Sink opened in write mode");

            lock (_syncRoot)
            {
                if (_fileStream.Position >= _fileStream.Length)
                    throw new EndOfStreamException();

                // 先读取记录长度
                int recordLength = _reader.ReadInt32();
                byte[] recordBytes = _reader.ReadBytes(recordLength);
                var task = DeserializeTask(recordBytes);
                recordLength = _reader.ReadInt32();
                return task;
            }
        }

        #endregion

        #region 反向读取最新100条记录并过滤重复 Key

        /// <summary>
        /// 从文件末尾开始反向扫描，读取最新最多100条记录，
        /// 然后根据 (LineNumber, BatchNumber) 过滤，只保留每个 Key 的最新一条记录。
        /// </summary>
        /// <returns>过滤后的 PLCProcessTask 枚举集合</returns>
        public IEnumerable<PLCProcessTask> ReadLatestUniqueTasks(int count = 30)
        {
            if (_forWriting)
                throw new InvalidOperationException("Sink opened in write mode");

            List<PLCProcessTask> latestRecords = new List<PLCProcessTask>();
            lock (_syncRoot)
            {
                long pos = _fileStream.Length;
                // 反向扫描最多读取 count * 3 条记录
                int iCount = count * 3;
                while (pos > 0 && latestRecords.Count < iCount)
                {
                    // 每条记录前有4字节记录长度，确保有足够数据
                    if (pos < 4)
                        break;
                    // 定位到记录长度前缀位置
                    _fileStream.Seek(pos - 4, SeekOrigin.Begin);
                    byte[] lengthBytes = new byte[4];
                    int bytesRead = _fileStream.Read(lengthBytes, 0, 4);
                    if (bytesRead != 4)
                        break; // 数据错误
                    int recordLength = BitConverter.ToInt32(lengthBytes, 0);

                    long recordStart = pos - 4 - recordLength;
                    if (recordStart < 0)
                        break; // 数据错误

                    // 定位到记录起始位置
                    _fileStream.Seek(recordStart, SeekOrigin.Begin);
                    byte[] recordBytes = new byte[recordLength];
                    bytesRead = _fileStream.Read(recordBytes, 0, recordLength);
                    if (bytesRead != recordLength)
                        break; // 数据错误

                    // 反序列化记录
                    PLCProcessTask task = DeserializeTask(recordBytes);
                    latestRecords.Add(task);
                    // 更新 pos 到上一条记录的结尾
                    pos = recordStart - 4;
                }
            }

            // 由于是反向扫描，latestRecords 中第一个是最新记录
            // 过滤：对于每个 (LineNumber, BatchNumber) 只保留第一次出现（即最新）的记录
            Dictionary<(uint, uint), PLCProcessTask> uniqueTasks = new Dictionary<(uint, uint), PLCProcessTask>();
            foreach (var task in latestRecords)
            {
                var key = (task.LineNumber, task.BatchNumber);
                if (!uniqueTasks.ContainsKey(key))
                    uniqueTasks.Add(key, task);
            }
            return uniqueTasks.Values;
        }

        #endregion

        #region 辅助方法：反序列化记录

        /// <summary>
        /// 根据记录字节反序列化出 PLCProcessTask 对象。
        /// 写入时的字段顺序为：
        ///   LineNumber, BatchNumber, TrayPos, CreateTime.Ticks,
        ///   DeterminationMode, CurrentProduct, ProductBatchCode,
        ///   InspectionType, DetectionReason, Voltage, Current,
        ///   ProductsInfo 数组（先写入数组长度，再逐个写入各字段）。
        /// </summary>
        /// <param name="recordBytes">记录字节数组</param>
        /// <returns>PLCProcessTask 对象</returns>
        private PLCProcessTask DeserializeTask(byte[] recordBytes)
        {
            using (var ms = new MemoryStream(recordBytes))
            using (var br = new BinaryReader(ms, Encoding.UTF8))
            {
                // 按写入顺序依次读取
                uint lineNumber = br.ReadUInt32();
                uint batchNumber = br.ReadUInt32();
                uint trayPos = br.ReadUInt32();
                long ticks = br.ReadInt64();
                DateTime createTime = new DateTime(ticks);
                string determinationMode = br.ReadString();
                string currentProduct = br.ReadString();
                string productBatchCode = br.ReadString();
                InspectionTypeEnum inspectionType = (InspectionTypeEnum)br.ReadInt32();
                DetectionReasonEnum detectionReason = (DetectionReasonEnum)br.ReadInt32();
                double voltage = br.ReadDouble();
                double current = br.ReadDouble();

                // 利用构造函数创建对象（构造函数内部会初始化 ProductsInfo 数组，但随后覆盖）
                PLCProcessTask task = new PLCProcessTask(determinationMode, currentProduct, productBatchCode, inspectionType, detectionReason)
                {
                    LineNumber = lineNumber,
                    BatchNumber = batchNumber,
                    TrayPos = trayPos,
                    CreateTime = createTime,
                    Voltage = voltage,
                    Current = current
                };

                // 读取 ProductsInfo 数组
                int productCount = br.ReadInt32();
                task.ProductsInfo = new ProductInformation[productCount];
                for (int i = 0; i < productCount; i++)
                {
                    ProductInformation product = new ProductInformation();
                    product.Position = br.ReadInt32();
                    product.HasProduct = br.ReadBoolean();
                    product.SN = br.ReadString();
                    product.ResultStatus = (ResultDetectionStatus)br.ReadInt32();
                    product.ResultStatusString = br.ReadString();
                    product.CheckToken = br.ReadString();
                    product.ScanFail = br.ReadBoolean();
                    product.PredictionImagePath = br.ReadString();
                    product.BeJudgementUser = br.ReadBoolean();
                    bool mesUploadBool = br.ReadBoolean();
                    string mesUploadStr = br.ReadString();
                    product.MesUploadResult = (mesUploadBool, mesUploadStr);
                    task.ProductsInfo[i] = product;
                }
                return task;
            }
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_forWriting)
                {
                    _writer?.Dispose();
                }
                else
                {
                    _reader?.Dispose();
                }
                _fileStream?.Dispose();
            }
        }

        #endregion

        public static void Test()
        {
            int testCount = 10; // 测试写入/读取的任务数量
            Random rand = new Random();

            // 生成测试数据
            PLCProcessTask[] tasks = new PLCProcessTask[testCount];
            for (int i = 0; i < testCount; i++)
            {
                tasks[i] = new PLCProcessTask("AI", "Product" + i, "Batch" + i, InspectionTypeEnum.GluePathInspection, DetectionReasonEnum.Normal)
                {
                    LineNumber = (uint)rand.Next(0, 2),
                    BatchNumber = (uint)i,
                    TrayPos = (uint)rand.Next(1, 10),
                    CreateTime = DateTime.Now,
                    Voltage = rand.NextDouble() * 10,
                    Current = rand.NextDouble() * 5
                };

                for (int j = 0; j < tasks[i].ProductsInfo.Length; j++)
                {
                    tasks[i].ProductsInfo[j].HasProduct = rand.Next(0, 2) == 1;
                    tasks[i].ProductsInfo[j].SN = "SN" + i + "-" + j;
                    tasks[i].ProductsInfo[j].ResultStatus = (ResultDetectionStatus)rand.Next(0, 3);
                }
            }

            // 测试写入
            Stopwatch sw = Stopwatch.StartNew();
            using (var sink = new PLCProcessTaskSink(false))
            {
                foreach (var task in tasks)
                {
                    sink.Write(task);
                }
                sink.Flush();
            }
            sw.Stop();
            Console.WriteLine($"写入 {testCount} 条记录耗时: {sw.ElapsedMilliseconds} ms");

            // 测试读取
            sw.Restart();
            using (var sink = new PLCProcessTaskSink(FileAccess.Read))
            {
                for (int i = 0; i < testCount; i++)
                {
                    PLCProcessTask task = sink.ReadNext();
                    if (task.BatchNumber != tasks[i].BatchNumber ||
                        task.CurrentProduct != tasks[i].CurrentProduct ||
                        task.LineNumber != tasks[i].LineNumber ||
                        task.TrayPos != tasks[i].TrayPos ||
                        task.Voltage != tasks[i].Voltage ||
                        task.Current != tasks[i].Current)
                    {
                        Console.WriteLine("数据不匹配，测试失败！");
                        return;
                    }
                    for (int j = 0; j < task.ProductsInfo.Length; j++)
                    {
                        if (task.ProductsInfo[j].HasProduct != tasks[i].ProductsInfo[j].HasProduct ||
                            task.ProductsInfo[j].SN != tasks[i].ProductsInfo[j].SN ||
                            task.ProductsInfo[j].ResultStatus != tasks[i].ProductsInfo[j].ResultStatus)
                        {
                            Console.WriteLine("产品数据不匹配，测试失败！");
                            return;
                        }
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"读取 {testCount} 条记录耗时: {sw.ElapsedMilliseconds} ms");

            // 测试反向读取最新100条记录并过滤重复 Key 并匹配
            sw.Restart();
            using (var sink = new PLCProcessTaskSink(FileAccess.Read))
            {
                var latestUniqueTasks = sink.ReadLatestUniqueTasks().ToList();
                Console.WriteLine($"读取最新100条记录并过滤后得到 {latestUniqueTasks.Count} 条唯一任务");

                foreach (var task in latestUniqueTasks)
                {
                    var matchingTask = tasks.FirstOrDefault(t => t.LineNumber == task.LineNumber && t.BatchNumber == task.BatchNumber);
                    if (matchingTask == null ||
                        task.CurrentProduct != matchingTask.CurrentProduct ||
                        task.TrayPos != matchingTask.TrayPos ||
                        task.Voltage != matchingTask.Voltage ||
                        task.Current != matchingTask.Current)
                    {
                        Console.WriteLine("反向读取数据不匹配，测试失败！");
                        return;
                    }
                    for (int j = 0; j < task.ProductsInfo.Length; j++)
                    {
                        if (task.ProductsInfo[j].HasProduct != matchingTask.ProductsInfo[j].HasProduct ||
                            task.ProductsInfo[j].SN != matchingTask.ProductsInfo[j].SN ||
                            task.ProductsInfo[j].ResultStatus != matchingTask.ProductsInfo[j].ResultStatus)
                        {
                            Console.WriteLine("反向读取产品数据不匹配，测试失败！");
                            return;
                        }
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"反向读取并过滤匹配测试耗时: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("测试通过，数据一致！");
        }
    }
}
