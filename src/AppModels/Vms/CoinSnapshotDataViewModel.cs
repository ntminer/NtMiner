using NTMiner.Core;
using NTMiner.Core.MinerServer;
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
                if (_data.CoinCode != value) {
                    _data.CoinCode = value;
                    OnPropertyChanged(nameof(CoinCode));
                }
            }
        }
        public double Speed {
            get => _data.Speed;
            set {
                if (_data.Speed != value) {
                    _data.Speed = value;
                    OnPropertyChanged(nameof(Speed));
                    OnPropertyChanged(nameof(SpeedText));
                }
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
                if (_data.MainCoinOnlineCount != value) {
                    _data.MainCoinOnlineCount = value;
                    OnPropertyChanged(nameof(MainCoinOnlineCount));
                }
            }
        }
        public int MainCoinMiningCount {
            get => _data.MainCoinMiningCount;
            set {
                if (_data.MainCoinMiningCount != value) {
                    _data.MainCoinMiningCount = value;
                    OnPropertyChanged(nameof(MainCoinMiningCount));
                }
            }
        }

        public int DualCoinOnlineCount {
            get => _data.DualCoinOnlineCount;
            set {
                if (_data.DualCoinOnlineCount != value) {
                    _data.DualCoinOnlineCount = value;
                    OnPropertyChanged(nameof(DualCoinOnlineCount));
                }
            }
        }
        public int DualCoinMiningCount {
            get => _data.DualCoinMiningCount;
            set {
                if (_data.DualCoinMiningCount != value) {
                    _data.DualCoinMiningCount = value;
                    OnPropertyChanged(nameof(DualCoinMiningCount));
                }
            }
        }

        public DateTime Timestamp {
            get => _data.Timestamp;
            set {
                if (_data.Timestamp != value) {
                    _data.Timestamp = value;
                    OnPropertyChanged(nameof(Timestamp));
                }
            }
        }
    }
}
