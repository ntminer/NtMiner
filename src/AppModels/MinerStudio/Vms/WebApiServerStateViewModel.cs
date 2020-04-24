using NTMiner.ServerNode;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace NTMiner.MinerStudio.Vms {
    public class WebApiServerStateViewModel : ViewModelBase, IWebApiServerState {
        private string _description;
        private string _oSInfo;
        private CpuData _cpu;
        private List<WsServerNodeState> _wsServerNodes;
        private ulong _totalPhysicalMemory;
        private string _address;
        private double _cpuPerformance;
        private ulong _availablePhysicalMemory;

        public WebApiServerStateViewModel() {

        }

        public List<WsServerNodeState> WsServerNodes {
            get => _wsServerNodes;
            set {
                _wsServerNodes = value;
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
