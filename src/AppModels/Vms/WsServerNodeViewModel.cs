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
        private string _osInfo;
        private CpuData _cpu;
        private ulong _totalPhysicalMemory;
        private double _cpuPerformance;
        private ulong _availablePhysicalMemory;

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
            _osInfo = data.OSInfo;
            _cpu = data.Cpu;
            _totalPhysicalMemory = data.TotalPhysicalMemory;
            _availablePhysicalMemory = data.AvailablePhysicalMemory;
            _cpuPerformance = data.CpuPerformance;

        }

        public void Update(IVarWsServerNode data) {
            if (this.Address != data.Address) {
                throw new InvalidProgramException();
            }
            this.MinerClientWsSessionCount = data.MinerClientWsSessionCount;
            this.MinerStudioWsSessionCount = data.MinerStudioWsSessionCount;
            this.MinerClientSessionCount = data.MinerClientSessionCount;
            this.MinerStudioSessionCount = data.MinerStudioSessionCount;
            this.CpuPerformance = data.CpuPerformance;
            this.AvailablePhysicalMemory = data.AvailablePhysicalMemory;
        }

        public string Address {
            get => _address;
            set {
                if (_address != value) {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
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

        public string OSInfo {
            get { return _osInfo; }
            set {
                if (_osInfo != value) {
                    _osInfo = value;
                    OnPropertyChanged(nameof(OSInfo));
                }
            }
        }

        public CpuData Cpu {
            get => _cpu;
            set {
                if (_cpu != value) {
                    _cpu = value;
                    OnPropertyChanged(nameof(Cpu));
                }
            }
        }

        public ulong TotalPhysicalMemory {
            get => _totalPhysicalMemory;
            set {
                if (_totalPhysicalMemory != value) {
                    _totalPhysicalMemory = value;
                    OnPropertyChanged(nameof(TotalPhysicalMemory));
                }
            }
        }

        public double CpuPerformance {
            get => _cpuPerformance;
            set {
                if (_cpuPerformance != value) {
                    _cpuPerformance = value;
                    OnPropertyChanged(nameof(CpuPerformance));
                    OnPropertyChanged(nameof(CpuPerformanceText));
                }
            }
        }

        public string CpuPerformanceText {
            get {
                return (this.CpuPerformance * 100).ToString("f1") + " %";
            }
        }

        public ulong AvailablePhysicalMemory {
            get => _availablePhysicalMemory;
            set {
                if (_availablePhysicalMemory != value) {
                    _availablePhysicalMemory = value;
                    OnPropertyChanged(nameof(AvailablePhysicalMemory));
                }
            }
        }
    }
}
