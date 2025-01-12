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

        public static void Load()
        {
            // TODO
        }

        public static string ConfigFilePathDriver(string driverName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, DRIVER_DIR_NAME, $"{driverName}.yaml");
        }

        public static string ConfigFilePathDriverPoints(string driverName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"{driverName}.points.conl");
        }

        public static string ConfigFilePathNGNameExchangePoints(string driverName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"{driverName}.csv");
        }

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


        public static string ConfigFilePathModel(string pathName)
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, pathName);
        }


        public static string PermissionsFilePathSystem()
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"Permissions.yaml");
        }
        public static string DaemonFilFilePathSystem()
        {
            return AppHelper.JoinFullPath(CONFIG_DIR_NAME, $"Daemon.yaml");
        }

        public static string HalconMainHDevFilePath()
        {
            return AppHelper.JoinFullPath($"Main.hdev");
        }

    }


}
