using NTMiner.Core;
using System;

namespace NTMiner.Vms {
    public class ShareViewModel : ViewModelBase, ICoinShare {
        private int _rejectShareCount;
        private int _acceptShareCount;
        private DateTime _shareOn;

        public ShareViewModel(Guid coinId) {
            this.CoinId = coinId;
        }

        public void Update(ICoinShare share) {
            _acceptShareCount = share.AcceptShareCount;
            _rejectShareCount = share.RejectShareCount;
            _shareOn = share.ShareOn;
            OnPropertyChanged(nameof(AcceptShareCount));
            OnPropertyChanged(nameof(TotalShareCount));
            OnPropertyChanged(nameof(RejectPercent));
            OnPropertyChanged(nameof(RejectPercentText));
            OnPropertyChanged(nameof(RejectShareCount));
            OnPropertyChanged(nameof(ShareOn));
        }

        public Guid CoinId { get; private set; }

        public int TotalShareCount {
            get {
                return AcceptShareCount + RejectShareCount;
            }
        }

        public int AcceptShareCount {
            get {
                return _acceptShareCount;
            }
            set {
                if (_acceptShareCount != value) {
                    _acceptShareCount = value;
                    OnPropertyChanged(nameof(AcceptShareCount));
                    OnPropertyChanged(nameof(TotalShareCount));
                    OnPropertyChanged(nameof(RejectPercentText));
                }
            }
        }

        public double RejectPercent {
            get {
                if (TotalShareCount == 0) {
                    return 0;
                }
                return ((double)RejectShareCount / TotalShareCount) * 100;
            }
        }

        public string RejectPercentText {
            get {
                return RejectPercent.ToString("f1") + "%";
            }
        }

        public int RejectShareCount {
            get => _rejectShareCount;
            set {
                if (_rejectShareCount != value) {
                    _rejectShareCount = value;
                    OnPropertyChanged(nameof(RejectShareCount));
                    OnPropertyChanged(nameof(RejectPercent));
                    OnPropertyChanged(nameof(TotalShareCount));
                    OnPropertyChanged(nameof(RejectPercentText));
                }
            }
        }

        public DateTime ShareOn {
            get => _shareOn;
            set {
                if (_shareOn != value) {
                    _shareOn = value;
                    OnPropertyChanged(nameof(ShareOn));
                }
            }
        }
    }
}
