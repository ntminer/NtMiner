using NTMiner.ServerNode;
using NTMiner.Vms;
using System;

namespace NTMiner.MinerStudio.Vms {
    public class WsServerNodeStateViewModel : ViewModelBase, IWsServerNode {
        private string _address;
        private string _description;
        private int _minerClientWsSessionCount;
        private int _minerStudioWsSessionCount;
        private int _minerClientSessionCount;
        private int _minerStudioSessionCount;
        private string _osInfo;
        private ulong _totalPhysicalMemory;
        private double _cpuPerformance;
        private ulong _availablePhysicalMemory;
        private CpuData _cpu;
        private CpuDataViewModel _cpuVm;

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public WsServerNodeStateViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public WsServerNodeStateViewModel(IWsServerNode data) {
            _address = data.Address;
            _description = data.Description;
            _minerClientWsSessionCount = data.MinerClientWsSessionCount;
            _minerStudioWsSessionCount = data.MinerStudioWsSessionCount;
            _minerClientSessionCount = data.MinerClientSessionCount;
            _minerStudioSessionCount = data.MinerStudioSessionCount;
            _osInfo = data.OSInfo;
            _totalPhysicalMemory = data.TotalPhysicalMemory;
            _availablePhysicalMemory = data.AvailablePhysicalMemory;
            _cpuPerformance = data.CpuPerformance;
            _cpu = data.Cpu;
            _cpuVm = new CpuDataViewModel(data.Cpu);
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

        public CpuDataViewModel CpuVm {
            get { return _cpuVm; }
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
                return this.CpuPerformance.ToString("f1") + " %";
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
