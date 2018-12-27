using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.Vms {
    public class CoinSnapshotDataViewModel : ViewModelBase {
        private readonly CoinSnapshotData _data;

        public CoinSnapshotDataViewModel(CoinSnapshotData data) {
            _data = data;
        }

        public void Update(CoinSnapshotData data) {
            this.Speed = data.Speed;
            this.MainCoinMiningCount = data.MainCoinMiningCount;
            this.MainCoinOnlineCount = data.MainCoinOnlineCount;
            this.DualCoinMiningCount = data.DualCoinMiningCount;
            this.DualCoinOnlineCount = data.DualCoinOnlineCount;
            this.Timestamp = data.Timestamp;
        }

        public string CoinCode {
            get => _data.CoinCode;
            set {
                _data.CoinCode = value;
                OnPropertyChanged(nameof(CoinCode));
            }
        }
        public long Speed {
            get => _data.Speed;
            set {
                _data.Speed = value;
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(SpeedText));
            }
        }

        public string SpeedText {
            get {
                return this.Speed.ToUnitSpeedText();
            }
        }

        public int MainCoinOnlineCount {
            get => _data.MainCoinOnlineCount;
            set {
                _data.MainCoinOnlineCount = value;
                OnPropertyChanged(nameof(MainCoinOnlineCount));
            }
        }
        public int MainCoinMiningCount {
            get => _data.MainCoinMiningCount;
            set {
                _data.MainCoinMiningCount = value;
                OnPropertyChanged(nameof(MainCoinMiningCount));
            }
        }

        public int DualCoinOnlineCount {
            get => _data.DualCoinOnlineCount;
            set {
                _data.DualCoinOnlineCount = value;
                OnPropertyChanged(nameof(DualCoinOnlineCount));
            }
        }
        public int DualCoinMiningCount {
            get => _data.DualCoinMiningCount;
            set {
                _data.DualCoinMiningCount = value;
                OnPropertyChanged(nameof(DualCoinMiningCount));
            }
        }

        public DateTime Timestamp {
            get => _data.Timestamp;
            set {
                _data.Timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }
    }
}
