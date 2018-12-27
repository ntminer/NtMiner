namespace NTMiner {
    public static class LongExtension {
        public static string ToUnitSpeedText(this long speed) {
            if (speed >= 10000000000000) {
                return (speed / 1000000000000d).ToString("f1") + " Th/s";
            }
            if (speed >= 10000000000) {
                return (speed / 1000000000d).ToString("f1") + " Gh/s";
            }
            if (speed >= 10000000) {
                return (speed / 1000000d).ToString("f1") + " Mh/s";
            }
            if (speed >= 10000) {
                return (speed / 1000d).ToString("f1") + " Kh/s";
            }
            return speed.ToString() + " H/s";
        }
    }
}
