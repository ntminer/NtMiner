using System;

namespace NTMiner {
    public static class Timestamp {
        public const int DesyncSeconds = 180;
        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1);

        public static ulong GetTimestamp() {
            return GetTimestamp(DateTime.Now.ToUniversalTime());
        }

        public static ulong GetTimestamp(DateTime dateTime) {
            return (ulong)(dateTime.ToUniversalTime() - UnixBaseTime).TotalSeconds;
        }

        public static DateTime FromTimestamp(ulong timestamp) {
            return UnixBaseTime.AddSeconds(timestamp).ToLocalTime();
        }
    }
}
