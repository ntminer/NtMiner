using NTMiner.ServerNode;
using System;

namespace NTMiner.Vms {
    public class WsServerNodeViewModel : ViewModelBase, IWsServerNode {
        private string _address;
        private string _description;
        private int _minerClientWsSessionCount;
        private int _minerStudioWsSessionCount;
        private int _minerClientSessionCount;
        private int _minerStudioSessionCount;
        private int _cpuPerformance;
        private int _totalPhysicalMemoryMb;
        private int _availablePhysicalMemoryMb;

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public WsServerNodeViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public WsServerNodeViewModel(IWsServerNode data) {
            _address = data.Address;
            _description = data.Description;
            _minerClientWsSessionCount = data.MinerClientWsSessionCount;
            _minerStudioWsSessionCount = data.MinerStudioWsSessionCount;
            _minerClientSessionCount = data.MinerClientSessionCount;
            _minerStudioSessionCount = data.MinerStudioSessionCount;
            _cpuPerformance = data.CpuPerformance;
            _totalPhysicalMemoryMb = data.TotalPhysicalMemoryMb;
            _availablePhysicalMemoryMb = data.AvailablePhysicalMemoryMb;
        }

        public void Update(IWsServerNode data) {
            if (this.Address != data.Address) {
                throw new InvalidProgramException();
            }
            this.Description = data.Description;
            this.MinerClientWsSessionCount = data.MinerClientWsSessionCount;
            this.MinerStudioWsSessionCount = data.MinerStudioWsSessionCount;
            this.MinerClientSessionCount = data.MinerClientSessionCount;
            this.MinerStudioSessionCount = data.MinerStudioSessionCount;
            this.CpuPerformance = data.CpuPerformance;
            this.TotalPhysicalMemoryMb = data.TotalPhysicalMemoryMb;
            this.AvailablePhysicalMemoryMb = data.AvailablePhysicalMemoryMb;
        }

        public string Address {
            get => _address;
            set {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public int MinerClientWsSessionCount {
            get => _minerClientWsSessionCount;
            set {
                if (_minerClientWsSessionCount != value) {
                    _minerClientWsSessionCount = value;
                    OnPropertyChanged(nameof(MinerClientWsSessionCount));
                }
            }
        }

        public int MinerStudioWsSessionCount {
            get => _minerStudioWsSessionCount;
            set {
                if (_minerStudioWsSessionCount != value) {
                    _minerStudioWsSessionCount = value;
                    OnPropertyChanged(nameof(MinerStudioWsSessionCount));
                }
            }
        }

        public int MinerClientSessionCount {
            get => _minerClientSessionCount;
            set {
                if (_minerClientSessionCount != value) {
                    _minerClientSessionCount = value;
                    OnPropertyChanged(nameof(MinerClientSessionCount));
                }
            }
        }

        public int MinerStudioSessionCount {
            get => _minerStudioSessionCount;
            set {
                if (_minerStudioSessionCount != value) {
                    _minerStudioSessionCount = value;
                    OnPropertyChanged(nameof(MinerStudioSessionCount));
                }
            }
        }

        public int CpuPerformance {
            get => _cpuPerformance;
            set {
                if (_cpuPerformance != value) {
                    _cpuPerformance = value;
                    OnPropertyChanged(nameof(CpuPerformance));
                }
            }
        }

        public int TotalPhysicalMemoryMb {
            get => _totalPhysicalMemoryMb;
            set {
                if (_totalPhysicalMemoryMb != value) {
                    _totalPhysicalMemoryMb = value;
                    OnPropertyChanged(nameof(TotalPhysicalMemoryMb));
                }
            }
        }

        public int AvailablePhysicalMemoryMb {
            get => _availablePhysicalMemoryMb;
            set {
                if (_availablePhysicalMemoryMb != value) {
                    _availablePhysicalMemoryMb = value;
                    OnPropertyChanged(nameof(AvailablePhysicalMemoryMb));
                }
            }
        }
    }
}
