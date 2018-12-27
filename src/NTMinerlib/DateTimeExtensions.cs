using System;

namespace NTMiner {
    public static class DateTimeExtensions {
        public static bool IsInTime(this DateTime time) {
            return Math.Abs((DateTime.Now - time).TotalSeconds) < 60;
        }
    }
}
