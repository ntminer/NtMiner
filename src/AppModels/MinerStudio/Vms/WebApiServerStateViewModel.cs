using NTMiner.ServerNode;
using NTMiner.Vms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

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
        private ObservableCollection<WsServerNodeStateViewModel> _wsServerNodeVms;

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
            _wsServerNodeVms = new ObservableCollection<WsServerNodeStateViewModel>(data.WsServerNodes.Select(a => new WsServerNodeStateViewModel(a)));
        }

        public void Update(IWebApiServerState data) {
            this.Description = data.Description;
            this.OSInfo = data.OSInfo;
            this.TotalPhysicalMemory = data.TotalPhysicalMemory;
            this.Address = data.Address;
            this.CpuPerformance = data.CpuPerformance;
            this.AvailablePhysicalMemory = data.AvailablePhysicalMemory;
            this.Cpu = data.Cpu;
            this.WsServerNodes = data.WsServerNodes;
        }

        public List<WsServerNodeState> WsServerNodes {
            get => _wsServerNodes;
            set {
                _wsServerNodes = value;
                if (value == null || value.Count == 0) {
                    _wsServerNodeVms.Clear();
                }
                else {
                    if (_wsServerNodeVms == null) {
                        _wsServerNodeVms = new ObservableCollection<WsServerNodeStateViewModel>(value.Select(a => new WsServerNodeStateViewModel(a)));
                    }
                    else {
                        var toRemoves = _wsServerNodeVms.Where(a => value.All(b => b.Address != a.Address)).ToArray();
                        foreach (var item in toRemoves) {
                            _wsServerNodeVms.Remove(item);
                        }
                        foreach (var item in value) {
                            var vm = _wsServerNodeVms.FirstOrDefault(a => a.Address == item.Address);
                            if (vm != null) {
                                vm.Update(item);
                            }
                            else {
                                _wsServerNodeVms.Add(new WsServerNodeStateViewModel(item));
                            }
                        }
                    }
                }
            }
        }

        public ObservableCollection<WsServerNodeStateViewModel> WsServerNodeVms {
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
                if (value != null) {
                    if (_cpuVm == null) {
                        _cpuVm = new CpuDataViewModel(value);
                    }
                    else {
                        _cpuVm.Update(value);
                    }
                }
                else {
                    _cpuVm = null;
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
