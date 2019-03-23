namespace NTMiner.Vms {
    public class MinuteItem {
        public MinuteItem(int minutes) {
            this.Minutes = minutes;
        }

        public int Minutes { get; set; }
        public string Text {
            get {
                if (this.Minutes == 0) {
                    return "不限";
                }
                return this.Minutes + "分钟内";
            }
        }
    }
}
