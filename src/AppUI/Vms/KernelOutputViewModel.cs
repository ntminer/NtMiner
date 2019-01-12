using NTMiner.Core.Kernels;
using System;

namespace NTMiner.Vms {
    public class KernelOutputViewModel : ViewModelBase, IKernelOutput {
        private Guid _id;
        private string _name;
        private string _totalSpeedPattern;
        private string _totalSharePattern;
        private string _acceptSharePattern;
        private string _gpuSpeedPattern;
        private string _rejectSharePattern;
        private string _rejectPercentPattern;

        private string _dualTotalSpeedPattern;
        private string _dualTotalSharePattern;
        private string _dualAcceptSharePattern;
        private string _dualGpuSpeedPattern;
        private string _dualRejectSharePattern;
        private string _dualRejectPercentPattern;

        public KernelOutputViewModel(IKernelOutput data) : this(data.GetId()) {
            _name = data.Name;
            _gpuSpeedPattern = data.GpuSpeedPattern;
            _rejectSharePattern = data.RejectSharePattern;
            _totalSharePattern = data.TotalSharePattern;
            _acceptSharePattern = data.AcceptSharePattern;
            _rejectPercentPattern = data.RejectPercentPattern;
            _totalSpeedPattern = data.TotalSpeedPattern;
            _dualGpuSpeedPattern = data.DualGpuSpeedPattern;
            _dualRejectSharePattern = data.DualRejectSharePattern;
            _dualAcceptSharePattern = data.DualAcceptSharePattern;
            _dualRejectPercentPattern = data.DualRejectPercentPattern;
            _dualTotalSharePattern = data.DualTotalSharePattern;
            _dualTotalSpeedPattern = data.DualTotalSpeedPattern;
        }

        public KernelOutputViewModel(Guid id) {
            this._id = id;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string TotalSpeedPattern {
            get => _totalSpeedPattern;
            set {
                _totalSpeedPattern = value;
                OnPropertyChanged(nameof(TotalSpeedPattern));
            }
        }

        public string TotalSharePattern {
            get { return _totalSharePattern; }
            set {
                _totalSharePattern = value;
                OnPropertyChanged(nameof(TotalSharePattern));
            }
        }

        public string AcceptSharePattern {
            get { return _acceptSharePattern; }
            set {
                _acceptSharePattern = value;
                OnPropertyChanged(nameof(AcceptSharePattern));
            }
        }

        public string RejectSharePattern {
            get { return _rejectSharePattern; }
            set {
                _rejectSharePattern = value;
                OnPropertyChanged(nameof(RejectSharePattern));
            }
        }

        public string RejectPercentPattern {
            get { return _rejectPercentPattern; }
            set {
                _rejectPercentPattern = value;
                OnPropertyChanged(nameof(RejectPercentPattern));
            }
        }

        public string GpuSpeedPattern {
            get => _gpuSpeedPattern;
            set {
                _gpuSpeedPattern = value;
                OnPropertyChanged(nameof(GpuSpeedPattern));
            }
        }

        public string DualTotalSpeedPattern {
            get => _dualTotalSpeedPattern;
            set {
                _dualTotalSpeedPattern = value;
                OnPropertyChanged(nameof(DualTotalSpeedPattern));
            }
        }

        public string DualTotalSharePattern {
            get { return _dualTotalSharePattern; }
            set {
                _dualTotalSharePattern = value;
                OnPropertyChanged(nameof(DualTotalSharePattern));
            }
        }

        public string DualAcceptSharePattern {
            get { return _dualAcceptSharePattern; }
            set {
                _dualAcceptSharePattern = value;
                OnPropertyChanged(nameof(DualAcceptSharePattern));
            }
        }

        public string DualRejectSharePattern {
            get { return _dualRejectSharePattern; }
            set {
                _dualRejectSharePattern = value;
                OnPropertyChanged(nameof(DualRejectSharePattern));
            }
        }

        public string DualRejectPercentPattern {
            get { return _dualRejectPercentPattern; }
            set {
                _dualRejectPercentPattern = value;
                OnPropertyChanged(nameof(DualRejectPercentPattern));
            }
        }

        public string DualGpuSpeedPattern {
            get => _dualGpuSpeedPattern;
            set {
                _dualGpuSpeedPattern = value;
                OnPropertyChanged(nameof(DualGpuSpeedPattern));
            }
        }
    }
}
