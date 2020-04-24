using NTMiner.ServerNode;
using NTMiner.Vms;
using System.Collections.Generic;

namespace NTMiner.MinerStudio.Vms {
    public class WebApiServerStateViewModel : ViewModelBase, IWebApiServerState {
        private string _description;
        private string _oSInfo;
        private ulong _totalPhysicalMemory;
        private string _address;
        private double _cpuPerformance;
        private ulong _availablePhysicalMemory;
        private CpuData _cpu;
        private CpuDataViewModel _cpuVm;
        private List<WsServerNodeState> _wsServerNodes;
        private List<WsServerNodeStateViewModel> _wsServerNodeVms;

        public WebApiServerStateViewModel(IWebApiServerState data) {
            _description = data.Description;
            _oSInfo = data.OSInfo;
            _totalPhysicalMemory = data.TotalPhysicalMemory;
            _address = data.Address;
            _cpuPerformance = data.CpuPerformance;
            _availablePhysicalMemory = data.AvailablePhysicalMemory;
            _cpu = data.Cpu;
            _cpuVm = new CpuDataViewModel(data.Cpu);
            _wsServerNodes = data.WsServerNodes;
            _wsServerNodeVms = new List<WsServerNodeStateViewModel>();
            foreach (var item in data.WsServerNodes) {
                _wsServerNodeVms.Add(new WsServerNodeStateViewModel(item));
            }
        }

        public List<WsServerNodeState> WsServerNodes {
            get => _wsServerNodes;
            set {
                _wsServerNodes = value;
            }
        }

        public List<WsServerNodeStateViewModel> WsServerNodeVms {
            get {
                return _wsServerNodeVms;
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

        public string OSInfo {
            get => _oSInfo;
            set {
                if (_oSInfo != value) {
                    _oSInfo = value;
                    OnPropertyChanged(nameof(OSInfo));
                }
            }
        }

        public CpuData Cpu {
            get => _cpu;
            set {
                _cpu = value;
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

        public string Address {
            get => _address;
            set {
                if (_address != value) {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        public double CpuPerformance {
            get => _cpuPerformance;
            set {
                if (_cpuPerformance != value) {
                    _cpuPerformance = value;
                    OnPropertyChanged(nameof(CpuPerformance));
                }
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
