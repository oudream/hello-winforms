using Serilog;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;

namespace CxSystemConfiguration.Utilities
{
    public static class LogHelper
    {
        public delegate void LogEventHandler(LogLevel level, string logMessage);
        public static event LogEventHandler LogEvent;

        private static LogLevel currentLogLevel = LogLevel.Verbose;

        private static ConcurrentStack<LogEntry> _logStack = new ConcurrentStack<LogEntry>();
        private static int _logId = 0;

        private static bool isRunning;

        public static void InitLog()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
              .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
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
                }
                catch (Exception ex)
                {
                    // 记录异常
                    Console.WriteLine($"Exception : {ex.Message}");
                }

            })
            { IsBackground = true }.Start();
        }

        public static void Stop() { isRunning = false; }

        private static void ProcessLoglist()
        {
            List<LogEntry> logList = new List<LogEntry>();
            while (_logStack.TryPop(out var logEntry))
            {
                logList.Add(logEntry);
            }
            // 注意：在这里 logStack 已经为空
            logList.Sort((a, b) => a.LogId.CompareTo(b.LogId));
            foreach (var logEntry in logList)
            {
                var logLevel = logEntry.LogLevel;
                var message = logEntry.Message;
                //var logMessage = message.Replace("\r\n", "  ");
                var logMessage = message.Replace("\r\n", "  ");
                string logLevelString = "信息";
                if (logLevel >= currentLogLevel)
                {
                    // 使用 Serilog 输出日志
                    switch (logLevel)
                    {
                        case LogLevel.Verbose:
                            logLevelString = "空白";
                            Serilog.Log.Logger.Verbose(logMessage);
                            break;
                        case LogLevel.Debug:
                            logLevelString = "调试";
                            Serilog.Log.Logger.Debug(logMessage);
                            break;
                        case LogLevel.Information:
                            logLevelString = "信息";
                            Serilog.Log.Logger.Information(logMessage);
                            break;
                        case LogLevel.Warning:
                            logLevelString = "警告";
                            Serilog.Log.Logger.Warning(logMessage);
                            break;
                        case LogLevel.Error:
                            logLevelString = "错误";
                            Serilog.Log.Logger.Error(logMessage);
                            break;
                        case LogLevel.Fatal:
                            logLevelString = "致命错误";
                            Serilog.Log.Logger.Fatal(logMessage);
                            break;
                        default:
                            Serilog.Log.Logger.Information(logMessage);
                            break;
                    }
                    //
                    if (LogEvent != null)
                    {
                        logMessage = $"[{DateTime.Now:dd/HH:mm:ss.fff}] {logLevelString}: {message}\r\n";
                        LogEvent?.Invoke(logLevel, logMessage);
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

        private static LogLevel GetLogLevelFromMessage(string logMessage)
        {
            // 解析日志消息，获取日志级别
            foreach (var translation in LogLevelTranslations)
            {
                if (logMessage.Contains(translation.Key))
                {
                    return translation.Value;
                }
            }
            return LogLevel.Information; // 默认为 Information
        }

        public static void SetLogLevel(LogLevel logLevel)
        {
            currentLogLevel = logLevel;
        }

        public static void Information(string message)
        {
            Log(LogLevel.Information, message);
        }

        public static void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public static void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public static void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public static void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        private static void Log(LogLevel logLevel, string message)
        {
            Interlocked.Increment(ref _logId);
            var logEntry = new LogEntry { LogId = _logId, LogLevel = logLevel, Message = string.Copy(message) };
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
        public int LogId { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
    }

}
