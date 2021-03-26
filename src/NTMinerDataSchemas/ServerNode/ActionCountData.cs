namespace NTMiner.ServerNode {
    public class ActionCountData : IActionCount {
        public ActionCountData() { }

        public string ActionName { get; set; }

        public long Count { get; set; }
    }
}
