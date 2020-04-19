namespace NTMiner.Vms {
    public class SpeedUnitViewModel {
        public static readonly SpeedUnitViewModel HPerSecond = new SpeedUnitViewModel("H/s");
        public static readonly SpeedUnitViewModel KhPerSecond = new SpeedUnitViewModel("Kh/s");
        public static readonly SpeedUnitViewModel MhPerSecond = new SpeedUnitViewModel("Mh/s");
        public static readonly SpeedUnitViewModel GhPerSecond = new SpeedUnitViewModel("Gh/s");
        public static readonly SpeedUnitViewModel ThPerSecond = new SpeedUnitViewModel("Th/s");

        public static SpeedUnitViewModel GetSpeedUnitVm(string unit) {
            if (string.IsNullOrEmpty(unit)) {
                return HPerSecond;
            }
            switch (unit.ToLower()) {
                case "h/s":
                    return HPerSecond;
                case "kh/s":
                    return KhPerSecond;
                case "mh/s":
                    return MhPerSecond;
                case "gh/s":
                    return GhPerSecond;
                case "th/s":
                    return ThPerSecond;
                default:
                    return HPerSecond;
            }
        }

        private readonly string _unit;

        private SpeedUnitViewModel(string unit) {
            _unit = unit;
        }

        public string Unit {
            get {
                return _unit;
            }
        }
    }
}
