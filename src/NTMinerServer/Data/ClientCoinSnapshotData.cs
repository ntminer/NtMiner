using NTMiner.MinerClient;
using System;

namespace NTMiner.Data {
    public class ClientCoinSnapshotData {
        private string _coinCode;

        public ClientCoinSnapshotData() { }

        public static ClientCoinSnapshotData Create(SpeedData speedData, out ClientCoinSnapshotData dualCoinSnapshotData) {
            bool isMainCoin = !string.IsNullOrEmpty(speedData.MainCoinCode);
            dualCoinSnapshotData = null;
            if (isMainCoin) {
                bool hasDualCoin = !string.IsNullOrEmpty(speedData.DualCoinCode) && speedData.DualCoinCode != speedData.MainCoinCode;
                if (hasDualCoin) {
                    dualCoinSnapshotData = new ClientCoinSnapshotData {
                        CoinCode = speedData.DualCoinCode,
                        ShareDelta = speedData.DualCoinShareDelta,
                        RejectShareDelta = speedData.DualCoinRejectShareDelta,
                        Speed = speedData.DualCoinSpeed,
                        Timestamp = DateTime.Now,
                        ClientId = speedData.ClientId
                    };
                }
                return new ClientCoinSnapshotData {
                    CoinCode = speedData.MainCoinCode,
                    ShareDelta = speedData.MainCoinShareDelta,
                    RejectShareDelta = speedData.MainCoinRejectShareDelta,
                    Speed = speedData.MainCoinSpeed,
                    Timestamp = DateTime.Now,
                    ClientId = speedData.ClientId
                };
            }
            return null;
        }

        public int Id { get; set; }
        public Guid ClientId { get; set; }
        public string CoinCode {
            get => _coinCode ?? string.Empty;
            set => _coinCode = value;
        }
        public double Speed { get; set; }
        public int ShareDelta { get; set; }
        public int RejectShareDelta { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
