using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxWorkStation.Utilities
{
    public static class TimeHelper
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // 获取当前时间戳
        public static long GetNow()
        {
            //return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        }

        public static long GetMs(DateTime dt)
        {
            //return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return new DateTimeOffset(dt).ToUnixTimeMilliseconds();
        }

        public static DateTime GetDateTime(long ms)
        {
            // 将Unix时间戳转换为DateTime
            var dateTime = UnixEpoch.AddMilliseconds(ms).ToLocalTime();

            // 将DateTime格式化为字符串
            return dateTime;
        }

        // 时间戳（毫秒数）转日期
        public static string GetDateTimeString(long ms, string format = "yyyy/MM/dd HH:mm:ss:fff")
        {
            // 将DateTime格式化为字符串
            return GetDateTime(ms).ToString(format);
        }

        public static string GetDateTimeString(DateTime dt, string format = "yyyy/MM/dd HH:mm:ss")
        {
            // 将DateTime格式化为字符串
            return dt.ToString(format);
        }

        public static string GetNowTimeString()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime.ToString("HH:mm:ss.fff");
        }

        public static bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
        {
            // 定义可能的日期时间格式
            string[] formats = new string[]
            {
            "yyyy-MM-dd HH:mm:ss.fff", // 包括毫秒
            "yyyy-MM-dd HH:mm:ss:fff", // 包括毫秒
            "yyyy-MM-dd HH:mm:ss",     // 不包括毫秒
            "yyyy/MM/dd HH:mm:ss:fff", // 使用斜杠分隔符，包括毫秒
            "yyyy/MM/dd HH:mm:ss.fff", // 使用斜杠分隔符，包括毫秒
            "yyyy/MM/dd HH:mm:ss",     // 使用斜杠分隔符，不包括毫秒
            "yyyyMMddHHmmss",
                                       // 在这里可以根据需要添加更多格式
            };

            // 尝试解析字符串
            return DateTime.TryParseExact(dateTimeString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
    }
}
