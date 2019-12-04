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
    }
}
