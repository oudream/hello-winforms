using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonInterfaces
{
    public static class CompressionHelper
    {
        // 打包整个目录
        public static async Task<string> CreateZipFileAsync(string zipDirPath, string destinationZipPath)
        {
            try
            {
                // 取目录路径
                if (!Directory.Exists(zipDirPath))
                {
                    return $"打包数据 错误 目录路径不存在: {zipDirPath}";
                }

                Stopwatch stopwatch = Stopwatch.StartNew();

                List<string> selectedFiles = new List<string>();

                if (FileSystemHelper.ScanAll(zipDirPath, out var zipFileInfos))
                {
                    for (int i = 0; i < zipFileInfos.Count; i++)
                    {
                        // 判断是否文件
                        if (zipFileInfos[i].Attributes.HasFlag(FileAttributes.Directory))
                        {
                            continue;
                        }
                        selectedFiles.Add(zipFileInfos[i].FullName);
                    }
                }

                // 打包
                await CreateZipFileAsync(selectedFiles, destinationZipPath, zipDirPath);

                stopwatch.Stop();
                var costImageProcessing = stopwatch.ElapsedMilliseconds;
                LogHelper.Debug($"打包数据 CreateZipFile Cost [{costImageProcessing}] ms\n");
                return string.Empty;
            }
            catch (Exception ex)
            {
                var message = $"打包数据 异常: {ex.Message}";
                LogHelper.Debug(message);
                return message;
            }
        }

        // 打包所选文件
        public static async Task CreateZipFileAsync(IEnumerable<string> files, string destinationZipFilePath, string baseFolder)
        {
            await Task.Run(() =>
            {
                if (File.Exists(destinationZipFilePath))
                {
                    File.Delete(destinationZipFilePath);
                }

                using (ZipArchive zip = ZipFile.Open(destinationZipFilePath, ZipArchiveMode.Create))
                {
                    HashSet<string> addedPaths = new HashSet<string>();

                    foreach (var file in files)
                    {
                        // 获取文件相对于基础文件夹的相对路径
                        string relativePath = GetRelativePath(baseFolder, file);

                        // 检查是否已经添加过该路径
                        if (!addedPaths.Contains(relativePath))
                        {
                            // 创建具有完整目录结构的条目
                            zip.CreateEntryFromFile(file, relativePath, CompressionLevel.Fastest);
                            addedPaths.Add(relativePath);
                        }
                    }
                }
            });
        }

        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

    }
}
