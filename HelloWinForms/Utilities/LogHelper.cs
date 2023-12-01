using Serilog;

namespace CxWorkStation.Utilities
{
    public static class LogHelper
    {
        public static void InitLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
        }

    }
}
