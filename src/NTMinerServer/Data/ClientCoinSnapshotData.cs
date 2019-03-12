using NTMiner.MinerClient;
using System;
using NTMiner.MinerServer;

namespace NTMiner.Data {
    public class ClientCoinSnapshotData {
        private string _coinCode;

        public ClientCoinSnapshotData() { }

        public static ClientCoinSnapshotData Create(IClientData clientData, SpeedData speedData,
            out ClientCoinSnapshotData dualCoinSnapshotData) {
            bool isMainCoin = !string.IsNullOrEmpty(speedData.MainCoinCode);
            dualCoinSnapshotData = null;
            if (isMainCoin) {
                bool hasDualCoin = !string.IsNullOrEmpty(speedData.DualCoinCode) &&
                                   speedData.DualCoinCode != speedData.MainCoinCode;
                if (hasDualCoin) {
                    int dualCoinShareDelta = 0;
                    int dualCoinRejectShareDelta = 0;
                    if (clientData.DualCoinCode == speedData.DualCoinCode) {
                        dualCoinShareDelta = (speedData.DualCoinTotalShare - speedData.DualCoinRejectShare) -
                                             (clientData.DualCoinTotalShare - clientData.DualCoinRejectShare);
                        dualCoinRejectShareDelta = speedData.DualCoinRejectShare - clientData.DualCoinRejectShare;
                    }

                    dualCoinSnapshotData = new ClientCoinSnapshotData {
                        CoinCode = speedData.DualCoinCode,
                        ShareDelta = dualCoinShareDelta,
                        RejectShareDelta = dualCoinRejectShareDelta,
                        Speed = speedData.DualCoinSpeed,
                        Timestamp = DateTime.Now,
                        ClientId = speedData.ClientId
                    };
                }

                int mainCoinShareDelta = 0;
                int mainCoinRejectShareDelta = 0;
                if (clientData.MainCoinCode == speedData.MainCoinCode) {
                    mainCoinShareDelta = (speedData.MainCoinRejectShare - speedData.MainCoinRejectShare) -
                                         (clientData.MainCoinTotalShare - clientData.MainCoinRejectShare);
                    mainCoinRejectShareDelta = speedData.MainCoinRejectShare - clientData.MainCoinRejectShare;
                }

                return new ClientCoinSnapshotData {
                    CoinCode = speedData.MainCoinCode,
                    ShareDelta = mainCoinShareDelta,
                    RejectShareDelta = mainCoinRejectShareDelta,
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
