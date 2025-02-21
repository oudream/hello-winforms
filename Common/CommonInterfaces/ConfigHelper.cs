using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommonInterfaces
{
    public static class ConfigHelper
    {
        // 常量，配置目录名
        public const string CONFIG_DIR_NAME = "Configs";

        // 常量，驱动目录名
        public const string DRIVER_DIR_NAME = "Drivers";  
        
        // 常量，日志目录名
        public const string LOG_DIR_NAME = "Logs";

        // 常量，临时文件目录名
        public const string TEMP_DIR_NAME = "Temp";

        // 常量，数据文件目录名
        public const string DATA_DIR_NAME = "Data";

        // 常量，点检文件目录名
        public const string SELFCHECK_DIR_NAME = "SelfCheck";

        // 常量，APP异常日志文件名
        public const string EXCEPTION_LOG_FILE_NAME = "异常日志.txt";

        // 常量，各模块版本记录文件
        public const string VERSION_LOG_FILE_NAME = "版本记录.csv";

        // 配置目录下的文件路径
        public static string ConfigFilePath(string fileName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, fileName);
        }

        // 日志目录下的文件路径
        public static string LogFilePath(string fileName)
        {
            return AppHelper.JoinFullPath(LOG_DIR_NAME, fileName);
        }

        // 驱动点表文件
        public static string ConfigFilePathDriverPoints(string driverName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"{driverName}.points.conl");
        }

        public static string ConfigFilePathNGNameExchangePoints(string driverName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"{driverName}.csv");
        }

        // 系统配置文件
        public static string ConfigFilePathSystem(int mode = 0)
        {
            //胶路模式
            if(mode == 1)
            {
                return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"SystemConfigs_胶路.yaml");
            }
            //水口模式
            else if (mode == 2)
            {
                return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"SystemConfigs_水口.yaml");
            }
            //无模式选择
            else
            {
                return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"SystemConfigs.yaml");
            }
        }

        // 客户端配置文件
        public static string ConfigFilePathClient()
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"ClientConfigs.yaml");
        }

        // 权限文件路径
        public static string PermissionsFilePathSystem()
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"Permissions.yaml");
        }

        // 守护进程配置文件路径
        public static string DaemonFilFilePathSystem()
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"Daemon.yaml");
        }

        // Halcon主文件路径
        public static string HalconMainHDevFilePath()
        {
            return AppHelper.JoinFullPath($"Main.hdev");
        }

        // 配方文件路径
        public static string ConfigFilePathProductRecipe()
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"Recipe.dat");
        }

    }


}
