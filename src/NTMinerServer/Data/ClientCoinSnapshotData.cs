using System;

namespace NTMiner.Data {
    public class ClientCoinSnapshotData {
        private string _coinCode;

        public ClientCoinSnapshotData() { }

        public int Id { get; set; }
        public Guid ClientId { get; set; }
        public bool IsMinig { get; set; }
        public string CoinCode {
            get => _coinCode ?? string.Empty;
            set => _coinCode = value;
        }
        public long Speed { get; set; }
        public int ShareDelta { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
