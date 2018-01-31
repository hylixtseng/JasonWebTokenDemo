using System;

namespace JasonWebTokenDemo.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 將 Unix 時間戳(秒數)轉換成 UTC 日期時間
        /// </summary>
        /// <param name="timestamp">Unix 時間戳(秒數)</param>
        /// <returns>UTC 日期時間</returns>
        public static DateTime ConvertFromUnixTimeSeconds(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// 將指定的 UTC 日期時間轉換成 Unix 時間戳(秒數)
        /// </summary>
        /// <param name="date">UTC 日期時間</param>
        /// <returns>Unix 時間戳(秒數)</returns>
        public static long ConvertToUnixTimeSeconds(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            // 再次確保轉到 UTC
            TimeSpan diff = date.ToUniversalTime() - origin;

            return (long)Math.Round(diff.TotalSeconds);
        }
    }
}
