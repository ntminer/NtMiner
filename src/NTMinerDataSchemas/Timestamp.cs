using System;

namespace NTMiner {
    public static class Timestamp {
        public const int DesyncSeconds = 180;
        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetTimestamp() {
            return GetTimestamp(DateTime.Now);
        }

        public static long GetTimestamp(DateTime dateTime) {
            var span = (dateTime.ToUniversalTime() - UnixBaseTime).TotalSeconds;
            return (long)span;
        }

        public static DateTime FromTimestamp(long timestamp) {
            return UnixBaseTime.AddSeconds(timestamp).ToLocalTime();
        }

        public static bool IsInTime(long timestamp) {
            long now = GetTimestamp(DateTime.Now);
            if (now > timestamp) {
                return timestamp + DesyncSeconds > now;
            }
            else {
                return now + DesyncSeconds > timestamp;
            }
        }

        public static string GetTimestampText(DateTime dateTime) {
            int offDay = (DateTime.Now.Date - dateTime.Date).Days;
            switch (offDay) {
                case 0:
                    return $"今天 {dateTime.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                case 1:
                    return $"昨天 {dateTime.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                case 2:
                    return $"前天 {dateTime.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                default:
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static string GetTimeSpanBeforeText(DateTime dateTime) {
            TimeSpan timeSpan = DateTime.Now - dateTime;
            if (timeSpan.Days >= 1) {
                return timeSpan.Days + " 天前";
            }
            if (timeSpan.Hours > 0) {
                return timeSpan.Hours + " 小时前";
            }
            if (timeSpan.Minutes > 2) {
                return timeSpan.Minutes + " 分钟前";
            }
            int totalSeconds = (int)timeSpan.TotalSeconds;
            if (totalSeconds < 0) {
                totalSeconds = 0;
            }
            return totalSeconds.ToString() + " 秒前";
        }

        public static string GetTimeSpanAfterText(int seconds) {
            return GetTimeSpanAfterText(TimeSpan.FromSeconds(seconds));
        }

        public static string GetTimeSpanAfterText(TimeSpan timeSpan) {
            if (timeSpan.Days >= 1) {
                return timeSpan.Days + " 天后";
            }
            if (timeSpan.Hours > 0) {
                return timeSpan.Hours + " 小时后";
            }
            if (timeSpan.Minutes > 2) {
                return timeSpan.Minutes + " 分钟后";
            }
            int totalSeconds = (int)timeSpan.TotalSeconds;
            if (totalSeconds < 0) {
                totalSeconds = 0;
            }
            return totalSeconds.ToString() + " 秒后";
        }
    }
}
