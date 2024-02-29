using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloServiceController : Form
    {
        public HelloServiceController()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string serviceName = "infer-Server";
            ServiceController service = new ServiceController(serviceName);

            try
            {
                if (service.Status != ServiceControllerStatus.Running)
                {
                    ShowMessage("服务正在启动...");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    ShowMessage("服务已成功启动。");
                }
                else
                {
                    ShowMessage("服务已经在运行中。");
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage($"操作无效: {ex.Message}");
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                ShowMessage($"启动服务时超时: {ex.Message}");
            }
        }

        private void ShowMessage(string v)
        {
            this.richTextBox1.AppendText(v + "\r\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string serviceName = "infer-Server";
            ServiceController service = new ServiceController(serviceName);

            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    ShowMessage("服务正在停止...");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    ShowMessage("服务已成功停止。");
                }
                else
                {
                    ShowMessage("服务已经停止。");
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage($"操作无效: {ex.Message}");
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                ShowMessage($"停止服务时超时: {ex.Message}");
            }
        }

        public static List<string> GetDirectoryList(string rootPath)
        {
            List<string> directoryList = new List<string>();
            try
            {
                // 获取指定目录下所有子目录的路径
                string[] subdirectoryEntries = Directory.GetDirectories(rootPath);

                foreach (string subdirectory in subdirectoryEntries)
                {
                    directoryList.Add(subdirectory);
                }
            }
            catch (Exception)
            {
            }
            return directoryList;
        }

        public static List<string> PrintDirectoryTree(DirectoryNode node, string indent = "")
        {
            List<string> lines = new List<string>();
            lines.Add($"{indent}[{node.Name}] 修改时间: {node.LastWriteTime}");

            foreach (var subdir in node.Subdirectories)
            {
                lines.AddRange(PrintDirectoryTree(subdir, indent + "  "));
            }

            foreach (var file in node.Files)
            {
                // 检查是否为快捷方式
                string shortcutIndicator = file.IsShortcut ? " (快捷方式)" : "";
                lines.Add($"{indent}  {file.Name}, 大小: {file.Size}字节, 修改时间: {file.LastWriteTime}, 类型: {file.FileType}{shortcutIndicator}");
            }

            return lines;
        }

        public static DirectoryNode BuildDirectoryTree(string rootPath)
        {
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            if (!rootDir.Exists)
            {
                throw new DirectoryNotFoundException($"目录 '{rootPath}' 不存在");
            }

            DirectoryNode rootNode = new DirectoryNode(rootDir.Name, rootDir.LastWriteTime);
            BuildTree(rootDir, rootNode);
            return rootNode;
        }

        private static void BuildTree(DirectoryInfo directoryInfo, DirectoryNode currentNode)
        {
            foreach (var dir in directoryInfo.GetDirectories())
            {
                DirectoryNode childNode = new DirectoryNode(dir.Name, dir.LastWriteTime);
                currentNode.Subdirectories.Add(childNode);
                BuildTree(dir, childNode); // 递归调用
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                FileInfoNode fileInfo = new FileInfoNode(file.Name, file.Length, file.LastWriteTime, file.Extension);
                currentNode.Files.Add(fileInfo);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string rootPath = @"E:\image\FastDeploy\example\build\Release"; // 替换为你的根目录路径
            try
            {
                var directoryTree = BuildDirectoryTree(rootPath);
                List<string> directoryList = PrintDirectoryTree(directoryTree);
                // 现在你可以按需处理 directoryList
                foreach (var line in directoryList)
                {
                    ShowMessage(line);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"错误: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //HelloGetFiles();
            //HelloScan();
            HelloCopyDirectory();
        }

        private void HelloGetFiles()
        {
            string rootPath = @"E:\image\FastDeploy\example\build\Release"; // 替换为你的根目录路径
            //string rootPath = @".";
            //var directoryList = GetDirectoryList(rootPath);
            var directoryList = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories).ToList();
            foreach (var dir in directoryList)
            {
                ShowMessage(dir);
            }
        }

        private void HelloScan()
        {
            string rootPath = @"E:\image\FastDeploy\example\build\Release"; // 替换为你的根目录路径
            //string rootPath = @".";
            if (TryScanAll(rootPath, out List<FileSystemInfo> items))
            {
                foreach (var item in items)
                {
                    ShowMessage($"{item.GetType().Name}: {item.FullName}");
                }
            }
            else
            {
                ShowMessage("目录扫描失败，可能是因为路径不存在或没有访问权限。");
            }
        }

        private void HelloCopyDirectory()
        {
            string sourcePath = @"E:\image\FastDeploy\example\build\Release\products\锡球1C\model1";
            string destinationPath = @"E:\image\FastDeploy\example\build\Release\model1";

            if (Directory.Exists(destinationPath))
            {
                if (DeleteDirectory(destinationPath))
                {
                    ShowMessage("目录删除成功。");
                }
            }

            bool result = TryCopyDirectory(sourcePath, destinationPath, true);
            if (result)
            {
                ShowMessage("目录复制成功。");
            }
            else
            {
                ShowMessage("目录复制失败。");
            }
        }

        public static bool DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    // 删除目录及其所有子目录和文件
                    Directory.Delete(path, recursive: true);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static void CopyDirectory(string sourceDirPath, string destDirPath, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"源目录不存在或无法找到: {sourceDirPath}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirPath, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirPath, subdir.Name);
                    CopyDirectory(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static bool TryCopyDirectory(string sourceDirPath, string destDirPath, bool copySubDirs = true)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirPath);

            // 检查源目录是否存在
            if (!sourceDir.Exists)
            {
                return false;
            }

            // 尝试创建目标目录
            DirectoryInfo destDir = null;
            try
            {
                if (!Directory.Exists(destDirPath))
                {
                    destDir = Directory.CreateDirectory(destDirPath);
                }
                else
                {
                    destDir = new DirectoryInfo(destDirPath);
                }
            }
            catch
            {
                return false; // 创建目标目录失败
            }

            bool success = true;

            try
            {
                // 复制文件
                foreach (FileInfo file in sourceDir.GetFiles())
                {
                    try
                    {
                        file.CopyTo(Path.Combine(destDirPath, file.Name), true);
                    }
                    catch
                    {
                        success = false;
                        break; // 停止进一步复制
                    }
                }

                // 递归复制子目录
                if (success && copySubDirs)
                {
                    foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
                    {
                        if (!TryCopyDirectory(subdir.FullName, Path.Combine(destDirPath, subdir.Name), copySubDirs))
                        {
                            success = false;
                            break; // 停止进一步复制
                        }
                    }
                }
            }
            catch
            {
                success = false;
            }

            // 如果复制失败，尝试回滚
            if (!success)
            {
                try
                {
                    destDir.Delete(true); // 尝试删除目标目录及其所有内容
                }
                catch
                {
                    // 如果回滚失败，可能需要手动介入
                }
            }

            return success;
        }

        public static List<FileSystemInfo> ScanAll(string rootPath)
        {
            List<FileSystemInfo> items = new List<FileSystemInfo>();
            ScanDirectory(new DirectoryInfo(rootPath), items);
            return items;
        }

        public static bool TryScanAll(string rootPath, out List<FileSystemInfo> items)
        {
            items = new List<FileSystemInfo>();
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            if (!rootDir.Exists)
            {
                return false;
            }
            try
            {
                ScanDirectory(rootDir, items);
                return true;
            }
            catch (Exception)
            {
                // 在失败的情况下，清除items并返回false
                items = null;
                return false;
            }
        }

        private static void ScanDirectory(DirectoryInfo directory, List<FileSystemInfo> items)
        {
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException($"目录 '{directory.FullName}' 不存在");
            }

            try
            {
                // 添加当前目录信息
                items.Add(directory);

                // 获取当前目录下所有文件，并添加到列表中
                FileInfo[] files = directory.GetFiles();
                foreach (FileInfo file in files)
                {
                    items.Add(file);
                }

                // 获取当前目录下所有子目录，并递归扫描
                DirectoryInfo[] subDirectories = directory.GetDirectories();
                foreach (DirectoryInfo subdir in subDirectories)
                {
                    ScanDirectory(subdir, items);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"没有权限访问目录 '{directory.FullName}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"在扫描目录 '{directory.FullName}' 时发生错误: {ex.Message}", ex);
            }
        }
    }

    public class FileInfoNode
    {
        public string Name { get; set; }
        public long Size { get; set; } // 文件大小，以字节为单位
        public DateTime LastWriteTime { get; set; }
        public string FileType { get; set; } // 文件扩展名

        public FileInfoNode(string name, long size, DateTime lastWriteTime, string fileType)
        {
            Name = name;
            Size = size;
            LastWriteTime = lastWriteTime;
            FileType = fileType;
        }

        public bool IsShortcut => FileType.ToLower() == ".lnk";

    }

    public class DirectoryNode
    {
        public string Name { get; set; }
        public DateTime LastWriteTime { get; set; } // 目录的修改时间
        public List<DirectoryNode> Subdirectories { get; set; } = new List<DirectoryNode>();
        public List<FileInfoNode> Files { get; set; } = new List<FileInfoNode>();

        public DirectoryNode(string name, DateTime lastWriteTime)
        {
            Name = name;
            LastWriteTime = lastWriteTime;
        }
    }

}
