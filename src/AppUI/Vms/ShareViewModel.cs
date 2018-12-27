using NTMiner.Core;
using System;

namespace NTMiner.Vms {
    public class ShareViewModel : ViewModelBase, ICoinShare {
        private int _rejectCount;
        private int _acceptShareCount;
        private DateTime _shareOn;

        public ShareViewModel(Guid coinId) {
            this.CoinId = coinId;
        }

        public Guid CoinId { get; private set; }

        public int TotalShareCount {
            get {
                return AcceptShareCount + RejectCount;
            }
        }

        public int AcceptShareCount {
            get {
                return _acceptShareCount;
            }
            set {
                _acceptShareCount = value;
                OnPropertyChanged(nameof(AcceptShareCount));
                OnPropertyChanged(nameof(TotalShareCount));
            }
        }

        public double RejectPercent {
            get {
                if (TotalShareCount == 0) {
                    return 0;
                }
                return ((double)RejectCount / TotalShareCount) * 100;
            }
        }

        public string RejectPercentText {
            get {
                return RejectPercent.ToString("f1") + "%";
            }
        }

        public int RejectCount {
            get => _rejectCount;
            set {
                _rejectCount = value;
                OnPropertyChanged(nameof(RejectCount));
                OnPropertyChanged(nameof(RejectPercent));
                OnPropertyChanged(nameof(TotalShareCount));
                OnPropertyChanged(nameof(RejectPercentText));
            }
        }

        public DateTime ShareOn {
            get => _shareOn;
            set {
                _shareOn = value;
                OnPropertyChanged(nameof(ShareOn));
            }
        }
    }
}
