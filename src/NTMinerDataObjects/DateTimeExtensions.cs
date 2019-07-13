using System;

namespace NTMiner {
    public static class DateTimeExtensions {
        public static bool IsInTime(this ulong time) {
            ulong now = Timestamp.GetTimestamp(DateTime.Now);
            if (now > time) {
                return time + Timestamp.DesyncSeconds > now;
            }
            else {
                return now + Timestamp.DesyncSeconds > time;
            }
        }

        public static ulong ToUlong(this DateTime time) {
            return Timestamp.GetTimestamp(time);
        }
    }
}
