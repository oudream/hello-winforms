using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.Utilities
{
    public static class StringHelper
    {
        public static string ExtractShortNameAfterDot(string fullName)
        {
            // 分割字符串，并获取最后一个部分
            var parts = fullName.Split('.');
            return parts.Length > 1 ? parts[parts.Length - 1] : fullName;
        }

        public static List<string> ExtractDirName(List<string> directoryList)
        {
            List<string> dirNameList = new List<string>();
            for (int i = 0; i < directoryList.Count; i++)
            {
                string fullPath = directoryList[i];
                dirNameList.Add(fullPath.Substring(fullPath.LastIndexOf(Path.DirectorySeparatorChar) + 1));
            }
            return directoryList;
        }

        public static string BytesToHexString(byte[] bytes, int offset=0, int count=-1)
        {
            if (count == -1 || count > bytes.Length - offset)
            {
                count = bytes.Length;
            }
            StringBuilder hex = new StringBuilder(count * 2);
            for (int i = offset; i < offset + count; i++)
            {
                hex.AppendFormat("{0:X2}", bytes[i]);
            }
            return hex.ToString();
        }

        public static byte[] HexStringToBytes(string hexString)
        {
            if (!string.IsNullOrEmpty(hexString) && hexString.Length % 2 == 0)
            {
                try
                {
                    byte[] bytes = new byte[hexString.Length / 2];
                    for (int i = 0; i < hexString.Length; i += 2)
                    {
                        string hexByte = hexString.Substring(i, 2);
                        bytes[i / 2] = Convert.ToByte(hexByte, 16);
                    }
                    return bytes;
                }
                catch (Exception)
                {
                }
            }
            return new byte[0];
        }

    }

}
