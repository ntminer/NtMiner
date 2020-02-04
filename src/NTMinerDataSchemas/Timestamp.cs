using System;

namespace NTMiner {
    public static class Timestamp {
        public const int DesyncSeconds = 180;
        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ulong GetTimestamp() {
            return GetTimestamp(DateTime.Now);
        }

        public static ulong GetTimestamp(DateTime dateTime) {
            var span = (dateTime.ToUniversalTime() - UnixBaseTime).TotalSeconds;
            if (span < 0) {
                return 0;
            }
            return (ulong)span;
        }

        public static DateTime FromTimestamp(ulong timestamp) {
            return UnixBaseTime.AddSeconds(timestamp).ToLocalTime();
        }

        public static bool IsInTime(ulong timestamp) {
            ulong now = GetTimestamp(DateTime.Now);
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
    }
}
