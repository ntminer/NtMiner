using System;

namespace NTMiner.Core {
    public static class SpeedDoubleExtension {
        public static string ToUnitSpeedText(this double speed) {
            ToUnitSpeedText(speed, out string speedValueText, out string speedUnit);
            return $"{speedValueText} {speedUnit}";
        }

        public static void ToUnitSpeedText(this double speed, out string speedValueText, out string speedUnit) {
            if (speed >= 10000000000000) {
                speedValueText = (speed / 1000000000000d).ToString("f1");
                speedUnit = "Th/s";
            }
            else if (speed >= 10000000000) {
                speedValueText = (speed / 1000000000d).ToString("f1");
                speedUnit = "Gh/s";
            }
            else if (speed >= 10000000) {
                speedValueText = (speed / 1000000d).ToString("f1");
                speedUnit = "Mh/s";
            }
            else if (speed >= 10000) {
                speedValueText = (speed / 1000d).ToString("f1");
                speedUnit = "Kh/s";
            }
            else {
                speedValueText = speed.ToString("f2");
                speedUnit = "H/s";
            }
        }

        public static double FromUnitSpeed(this double speed, string speedUnit) {
            speedUnit = speedUnit?.Trim().ToLower();
            if (!speedUnit.EndsWith("/s")) {
                return 0;
            }
            speedUnit = speedUnit.Substring(0, speedUnit.Length - 2);
            speedUnit = speedUnit.Trim();
            if (speedUnit.Length > 1 && double.TryParse(speedUnit.Substring(0, speedUnit.Length - 1), out double per)) {
                return speed * per;
            }
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

        public static double ToNearSpeed(this double speed, double nearIt) {
            if (nearIt == 0) {
                return speed;
            }
            if (speed == 0) {
                return 0;
            }
            int count = 0;
            while (nearIt >= 1000) {
                nearIt /= 1000;
                count++;
            }
            while (speed >= 1000) {
                speed /= 1000;
            }
            if (count > 0) {
                speed = speed * count *1000;
            }
            return speed;
        }

        /// <summary>
        /// Math.Abs(speed - newSpeed) / speed >= percnet
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="newSpeed"></param>
        /// <param name="minChangeScopPercent">以小数表示百分比，即1%是0.01</param>
        /// <returns></returns>
        public static bool IsChange(this double speed, double newSpeed, double minChangeScopPercent) {
            if (speed == 0 && newSpeed != 0) {
                return true;
            }
            return Math.Abs(speed - newSpeed) / speed >= minChangeScopPercent;
        }
    }
}
