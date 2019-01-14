namespace NTMiner {
    public static class DoubleExtension {
        public static string ToUnitSpeedText(this double speed) {
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
            return speed.ToString("f2") + " H/s";
        }

        public static double FromUnitSpeed(this double speed, string speedUnit) {
            speedUnit = speedUnit?.Trim().ToLower();
            if (!speedUnit.EndsWith("/s")) {
                return 0;
            }
            speedUnit = speedUnit.Substring(0, speedUnit.Length - 2);
            speedUnit = speedUnit.Trim();
            switch (speedUnit) {
                case "h":
                    return speed;
                case "kh":
                case "k":
                    return speed * 1000;
                case "mh":
                case "m":
                    return speed * 1000000;
                case "gh":
                case "g":
                    return speed * 1000000000;
                case "th":
                case "t":
                    return speed * 1000000000000;
                default:
                    return 0;
            }
        }
    }
}
