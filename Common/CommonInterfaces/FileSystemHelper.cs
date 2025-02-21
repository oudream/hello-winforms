﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    // 不抛异常
    public static class FileSystemHelper
    {
        // 获取路径的根目录
        public static string GetPathRoot(string path)
        {
            return Path.GetPathRoot(path);
        }

        // 获取路径的父目录
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        // 获取路径的文件名
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        // 获取路径的文件名(不包括扩展名)
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        // 获取路径的扩展名
        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        // 改变路径的扩展名
        public static string ChangeExtension(string path, string newExtension)
        {
            return Path.ChangeExtension(path, newExtension);
        }

        // 获取路径FileSystemInfo
        public static FileSystemInfo GetFileSystemInfo(string path)
        {
            // 判断路径是文件还是目录，据此返回FileInfo或DirectoryInfo对象
            try
            {
                if (File.Exists(path))
                {
                    return new FileInfo(path);
                }
                else if (Directory.Exists(path))
                {
                    return new DirectoryInfo(path);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        // 获取指定目录下所有子目录的路径
        // 如果rootPath是全路径，会返回全路径；如果rootPath是相对路径，则会返回相对路径
        public static List<string> GetSubDirectoryList(string rootPath)
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

        // 只获取指定目录下所有文件的路径
        // 如果rootPath是全路径，会返回全路径；如果rootPath是相对路径，则会返回相对路径
        public static List<string> GetSubFileList(string rootPath)
        {
            List<string> fileList = new List<string>();
            try
            {
                // 获取指定目录下所有子目录的路径
                string[] subfileEntries = Directory.GetFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly);

                foreach (string subfile in subfileEntries)
                {
                    fileList.Add(subfile);
                }
            }
            catch (Exception)
            {
            }
            return fileList;
        }

        // 获取指定目录（迭代）下所有文件的路径
        public static List<string> GetAllSubFileList(string rootPath)
        {
            List<string> fileList = new List<string>();
            try
            {
                // 获取指定目录下所有子目录的路径
                string[] subfileEntries = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);

                foreach (string subfile in subfileEntries)
                {
                    fileList.Add(subfile);
                }
            }
            catch (Exception)
            {
            }
            return fileList;
        }

        // 获取指定目录下所有子目录
        public static List<DirectoryInfo> GetSubDirectoryInfoList(string rootPath)
        {
            try
            {
                DirectoryInfo rootDir = new DirectoryInfo(rootPath);
                DirectoryInfo[] subDirectories = rootDir.GetDirectories();
                return subDirectories.ToList();
            }
            catch (Exception)
            {
            }
            return new List<DirectoryInfo>();
        }

        public static List<FileInfo> GetSubFileInfoList(string rootPath)
        {
            try
            {
                DirectoryInfo rootDir = new DirectoryInfo(rootPath);
                FileInfo[] subFiles = rootDir.GetFiles();
                return subFiles.ToList();
            }
            catch (Exception)
            {
            }
            return new List<FileInfo>();
        }

        // 扫描指定目录下所有文件和包括子目录，返回文件列表，包括文件夹。
        public static bool ScanAll(string rootPath, out List<FileSystemInfo> items)
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

        // 打印目录树
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

        // 构建目录树
        public static DirectoryNode BuildDirectoryTree(string rootPath)
        {
            try
            {
                DirectoryInfo rootDir = new DirectoryInfo(rootPath);
                if (!rootDir.Exists)
                {
                    return null;
                }

                DirectoryNode rootNode = new DirectoryNode(rootDir.Name, rootDir.LastWriteTime);
                BuildTree(rootDir, rootNode);
                return rootNode;
            }
            catch (Exception)
            {
            }
            return null;
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

        public static bool CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
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

        public static bool CopyDirectory(string sourceDirPath, string destDirPath, bool copySubDirs = true)
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
                        if (!CopyDirectory(subdir.FullName, Path.Combine(destDirPath, subdir.Name), copySubDirs))
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

        public static bool IsSubdirectory(string parentDir, string subDir)
        {
            var parentDirectoryInfo = new DirectoryInfo(parentDir).FullName; 
            var subDirectoryInfo = new DirectoryInfo(subDir).FullName; 
            return subDirectoryInfo.StartsWith(parentDirectoryInfo, StringComparison.OrdinalIgnoreCase);
        }

        public static string ReadAllText(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }
            catch (Exception)
            {
            }

            return string.Empty;
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
