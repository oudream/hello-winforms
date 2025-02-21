using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public static class MachineIdentifierHelper
    {
        public static string MechanicalId { get; set; } = string.Empty;

        public static uint MechanicalNumber { get; private set; } = 1101;

        public static string GetMechanicalId()
        {
            string cpuId = GetCpuId();
            string diskId = GetDiskId();
            //string macAddress = GetMacAddress();

            // 合并硬件信息生成唯一标识
            //string rawMachineId = $"{cpuId}-{diskId}-{macAddress}";
            string rawMachineId = $"{cpuId}-{diskId}";

            // 对标识进行哈希处理
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawMachineId));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
            }
        }

        private static string GetCpuId()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
            {
                foreach (var item in searcher.Get())
                {
                    return item["ProcessorId"]?.ToString() ?? "UNKNOWN";
                }
            }
            return "UNKNOWN";
        }

        private static string GetMotherboardId()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
            {
                foreach (var item in searcher.Get())
                {
                    return item["SerialNumber"]?.ToString() ?? "UNKNOWN";
                }
            }
            return "UNKNOWN";
        }

        private static string GetDiskId()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive"))
            {
                foreach (var item in searcher.Get())
                {
                    return item["SerialNumber"]?.ToString() ?? "UNKNOWN";
                }
            }
            return "UNKNOWN";
        }

        public static bool GetVerifyKey()
        {
            string cpuId = GetCpuId();
            using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive"))
            {
                foreach (var item in searcher.Get())
                {
                    string diskId = item["SerialNumber"]?.ToString() ?? "UNKNOWN";

                    string rawMachineId = $"{cpuId}-{diskId}";

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawMachineId));
                        string Userkey = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();

                        var subId = Userkey.Substring(0, 10);
                        MechanicalId = subId;

                        // 读取并解析 license.dat 文件
                        var licenseData = FileSystemHelper.ReadAllText(ConfigHelper.ConfigFilePath("license.dat"));
                        if (string.IsNullOrWhiteSpace(licenseData))
                        {
                            return false; // 如果文件为空，则验证失败
                        }

                        // 按逗号分割 license 和 machineNum
                        var licenseParts = licenseData.Split(',');
                        if (licenseParts.Length < 2)
                        {
                            return false; // 如果格式不正确，则验证失败
                        }

                        string license = licenseParts[0].Trim();
                        if (!uint.TryParse(licenseParts[1].Trim(), out uint machineNumber))
                        {
                            return false; // 如果 machineNum 不是有效的 uint，则验证失败
                        }
                        MechanicalNumber = machineNumber;

                        var pwd = MachineIdentifierHelper.GeneratePassword(subId);
                        if (license == pwd)
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }


        private static string GetMacAddress()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL"))
            {
                foreach (var item in searcher.Get())
                {
                    return item["MACAddress"]?.ToString() ?? "UNKNOWN";
                }
            }
            return "UNKNOWN";
        }

        public static string GeneratePassword(string machineId)
        {
            // 使用机器ID生成绑定密码
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineId));
                var b64 = Convert.ToBase64String(hashBytes);
                var reverse = StringHelper.Reverse(b64);
                return reverse.Substring(1, reverse.Length - 2);
            }
        }
    }
}
