using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CxWorkStation.Utilities
{
    public static class LogHelper
    {
        public delegate void LogEventHandler(LogLevel level, string logMessage);
        public static event LogEventHandler LogEvent;

        private static LogLevel currentLogLevel = LogLevel.Verbose;
        private static bool _serilogInit = false;

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
            if (_serilogInit == false)
            {
                Serilog.Log.Logger = new LoggerConfiguration()
               .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
               .CreateLogger();
                _serilogInit = true;
            }
            string logLevelString = "信息";
            if (logLevel >= currentLogLevel)
            {
                // 使用 Serilog 输出日志
                switch (logLevel)
                {
                    case LogLevel.Verbose:
                        logLevelString = "空白";
                        Serilog.Log.Logger.Verbose(message);
                        break;
                    case LogLevel.Debug:
                        logLevelString = "调试";
                        Serilog.Log.Logger.Debug(message);
                        break;
                    case LogLevel.Information:
                        logLevelString = "信息";
                        Serilog.Log.Logger.Information(message);
                        break;
                    case LogLevel.Warning:
                        logLevelString = "警告";
                        Serilog.Log.Logger.Warning(message);
                        break;
                    case LogLevel.Error:
                        logLevelString = "错误";
                        Serilog.Log.Logger.Error(message);
                        break;
                    case LogLevel.Fatal:
                        logLevelString = "致命错误";
                        Serilog.Log.Logger.Fatal(message);
                        break;
                    default:
                        Serilog.Log.Logger.Information(message);
                        break;
                }
                //
                if (LogEvent != null)
                {
                    string logMessage = $"[{DateTime.Now:dd/HH:mm:ss}] {logLevelString}: {message}\r\n";
                    LogEvent?.Invoke(logLevel, logMessage);
                }
            }
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

}
