using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace CommonInterfaces
{
    public static class LogHelper
    {
        public delegate void LogEventHandler(long time, LogLevel level, string logMessage, string logTag);
        public static event LogEventHandler LogEvent;

        private static LogLevel currentLogLevel = LogLevel.Verbose;

        private static ConcurrentStack<LogEntry> _logStack = new ConcurrentStack<LogEntry>();
        private static int _logId = 0;

        private static bool isRunning;
        /// <summary>
        /// 检查当前程序运行目录下是否有Logs文件夹,没有则创建
        /// </summary>
        /// <returns>返回Log文件夹路径</returns>
        private static string LogDirectoryCheck() 
        {
            string logDirPath = AppHelper.JoinFullPath("Logs");
            DirectoryInfo DirInfo=new DirectoryInfo(logDirPath);
            if (!DirInfo.Exists) 
            {
                try
                {
                    Directory.CreateDirectory(logDirPath);
                }
                catch (Exception ex)
                {
                    // 记录异常
                    Console.WriteLine($"Exception : {ex.Message}");
                    logDirPath = null;
                    throw;
                }
            }
            return logDirPath;
        }
        public static void Run(string LogFileName)
        {
            // 启动时检测Logs文件夹是否存在，创建失败会退出程序
            string logDir = LogDirectoryCheck();
            //防止Logs文件夹创建失败,不存在时则生成到当前程序运行目录
            string logPath = AppHelper.JoinFullPath(logDir, $"{LogFileName}-log-.txt");
            Serilog.Log.Logger = new LoggerConfiguration()
              .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
              .MinimumLevel.Verbose()
              .CreateLogger();

            // 初始化定时器，每50毫秒执行一次
            AutoResetEvent updateSignal = new AutoResetEvent(false);
            isRunning = true;
            new Thread(() =>
            {
                try
                {
                    while (isRunning)
                    {
                        // 等待一小段时间或直到收到更新信号
                        //_updateSignal.WaitOne(); // 等待更新信号
                        updateSignal.WaitOne(TimeSpan.FromMilliseconds(50));
                        // 
                        ProcessLoglist();
                    }

                    Serilog.Log.CloseAndFlush();
                }
                catch (Exception ex)
                {
                    // 记录异常
                    Console.WriteLine($"Exception : {ex.Message}");
                    throw;
                }

            })
            { IsBackground = true }.Start();
        }

        public static void Stop() 
        { 
            isRunning = false;
        }

        private static void ProcessLoglist()
        {
            List<LogEntry> logList = new List<LogEntry>();
            while (_logStack.TryPop(out var logEntry))
            {
                logList.Add(logEntry);
            }
            if (logList.Count <= 0) { return; }
            // 注意：在这里 logStack 已经为空
            logList.Sort((a, b) => a.Id.CompareTo(b.Id));
            foreach (var logEntry in logList)
            {
                var logLevel = logEntry.Level;
                var logTimestamp = logEntry.Timestamp;
                var message = logEntry.Message;
                //var logMessage = message.Replace("\r\n", "  ");
                var logTag = logEntry.Tag;
                var logMessage = message.Replace("\r\n", "  ");
                logMessage = $"[{logTag}] {logMessage}";
                //string logLevelString = "信息";
                if (logLevel >= currentLogLevel)
                {
                    // 使用 Serilog 输出日志
                    switch (logLevel)
                    {
                        case LogLevel.Verbose:
                            //logLevelString = "空白";
                            Serilog.Log.Logger.Verbose(logMessage);
                            break;
                        case LogLevel.Debug:
                            //logLevelString = "调试";
                            Serilog.Log.Logger.Debug(logMessage);
                            break;
                        case LogLevel.Information:
                            //logLevelString = "信息";
                            Serilog.Log.Logger.Information(logMessage);
                            break;
                        case LogLevel.Warning:
                            //logLevelString = "警告";
                            Serilog.Log.Logger.Warning(logMessage);
                            break;
                        case LogLevel.Error:
                            //logLevelString = "错误";
                            Serilog.Log.Logger.Error(logMessage);
                            break;
                        case LogLevel.Fatal:
                            //logLevelString = "致命错误";
                            Serilog.Log.Logger.Fatal(logMessage);
                            break;
                        default:
                            Serilog.Log.Logger.Information(logMessage);
                            break;
                    }
                    //
                    if (LogEvent != null)
                    {
                        //logMessage = $"[{DateTime.Now:dd/HH:mm:ss.fff}] {logLevelString}:{logTag}> {message}\r\n";
                        LogEvent?.Invoke(logTimestamp, logLevel, logMessage, logTag);
                    }
                }
            }
        }

        public static Dictionary<string, LogLevel> LogLevelTranslations = new Dictionary<string, LogLevel>
        {
            {"空白", LogLevel.Verbose},
            {"调试", LogLevel.Debug},
            {"信息", LogLevel.Information},
            {"警告", LogLevel.Warning},
            {"错误", LogLevel.Error},
            {"致命错误", LogLevel.Fatal},
        };

        public static string GetLogLevelTranslation(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return "空白";
                case LogLevel.Debug:
                    return "调试";
                case LogLevel.Information:
                    return "信息";
                case LogLevel.Warning:
                    return "警告";
                case LogLevel.Error:
                    return "错误";
                case LogLevel.Fatal:
                    return "致命错误";
                default:
                    return "";
            }
        }

        public static void SetLogLevel(LogLevel logLevel)
        {
            currentLogLevel = logLevel;
        }

        public static void Information(string message, string tag = "")
        {
            Log(LogLevel.Information, message, tag);
        }

        public static void Warning(string message, string tag = "")
        {
            Log(LogLevel.Warning, message, tag);
        }

        public static void Error(string message, string tag = "")
        {
            Log(LogLevel.Error, message, tag);
        }

        public static void Fatal(string message, string tag = "")
        {
            Log(LogLevel.Fatal, message, tag);
        }

        public static void Debug(string message, string tag="")
        {
            Log(LogLevel.Debug, message, tag);
        }

        public static void Log(LogLevel logLevel, string message, string tag = "")
        {
            Interlocked.Increment(ref _logId);
            var logEntry = new LogEntry { Id = _logId, Level = logLevel, Timestamp = TimeHelper.GetNow(), Message = string.Copy(message), Tag = string.Copy(tag) };
            // 推入新的日志消息
            _logStack.Push(logEntry);
        }
    }

    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }

    public class LogEntry
    {
        public int Id { get; set; }
        public LogLevel Level { get; set; }
        public long Timestamp { get; set; }
        public string Message { get; set; }
        public string Tag { get; set; }
    }

    public class LogItem
    {
        public string Text { get; set; }
        public Color Color { get; set; }
        public LogLevel Level { get; set; }
        public long Timestamp { get; set; }
        public List<string> Tags { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
