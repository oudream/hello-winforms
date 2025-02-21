using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public static class ProcessHelper
    {
        public static string GetProcessFullPath(string procName)
        {
            try
            {
                // 通过名称获取进程列表
                Process[] processes = Process.GetProcessesByName(procName);

                if (processes.Length > 0)
                {
                    // 这里仅取第一个同名进程
                    Process proc = processes[0];

                    // 获取进程可执行文件完整路径
                    string fullPath = proc.MainModule?.FileName;

                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        // 获取所在目录
                        //string directory = Path.GetDirectoryName(fullPath);

                        //Console.WriteLine($"{procName} 全路径: {fullPath}");
                        //Console.WriteLine($"{procName} 所在目录: {directory}");

                        return fullPath;
                    }
                    else
                    {
                        //Console.WriteLine($"无法获取进程 {procName} 的可执行文件全路径。");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"获取进程 {procName} 信息时出错: {ex.Message}");
            }
            return null;
        }
    }
}
