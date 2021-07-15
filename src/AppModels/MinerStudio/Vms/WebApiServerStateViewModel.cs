using NTMiner.ServerNode;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class WebApiServerStateViewModel : ViewModelBase, IWebApiServerState {
        private string _description;
        private string _oSInfo;
        private ulong _totalPhysicalMemory;
        private string _address;
        private double _cpuPerformance;
        private ulong _availablePhysicalMemory;
        private double _processMemoryMb;
        private long _threadCount;
        private long _handleCount;
        private string _availableFreeSpaceInfo;
        private int _captchaCount;
        private CpuData _cpu;
        private CpuDataViewModel _cpuVm;
        private List<WsServerNodeState> _wsServerNodes;
        private ObservableCollection<WsServerNodeStateViewModel> _wsServerNodeVms;
        private WsServerNodeStateViewModel _selectedItemVm;

        public WebApiServerStateViewModel(IWebApiServerState data) {
            _description = data.Description;
            _oSInfo = data.OSInfo;
            _totalPhysicalMemory = data.TotalPhysicalMemory;
            _address = data.Address;
            _cpuPerformance = data.CpuPerformance;
            _availablePhysicalMemory = data.AvailablePhysicalMemory;
            _processMemoryMb = data.ProcessMemoryMb;
            _threadCount = data.ThreadCount;
            _handleCount = data.HandleCount;
            _availableFreeSpaceInfo = data.AvailableFreeSpaceInfo;
            _captchaCount = data.CaptchaCount;
            _cpu = data.Cpu;
            _cpuVm = new CpuDataViewModel(data.Cpu);
            _wsServerNodes = data.WsServerNodes;
            _wsServerNodes.Sort((l, r) => string.Compare(l.Description, r.Description));
            _wsServerNodeVms = new ObservableCollection<WsServerNodeStateViewModel>(data.WsServerNodes.Select(a => new WsServerNodeStateViewModel(a)));
        }

        public void Update(IWebApiServerState data) {
            this.Description = data.Description;
            this.OSInfo = data.OSInfo;
            this.TotalPhysicalMemory = data.TotalPhysicalMemory;
            this.Address = data.Address;
            this.CpuPerformance = data.CpuPerformance;
            this.AvailablePhysicalMemory = data.AvailablePhysicalMemory;
            this.ProcessMemoryMb = data.ProcessMemoryMb;
            this.ThreadCount = data.ThreadCount;
            this.HandleCount = data.HandleCount;
            this.AvailableFreeSpaceInfo = data.AvailableFreeSpaceInfo;
            this.CaptchaCount = data.CaptchaCount;
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
                    bool needSort = false;
                    if (_wsServerNodeVms == null) {
                        needSort = true;
                        _wsServerNodeVms = new ObservableCollection<WsServerNodeStateViewModel>(value.Select(a => new WsServerNodeStateViewModel(a)));
                    }
                    else {
                        var toRemoves = _wsServerNodeVms.Where(a => value.All(b => b.Address != a.Address)).ToArray();
                        if (toRemoves.Length != 0) {
                            needSort = true;
                        }
                        foreach (var item in toRemoves) {
                            _ = _wsServerNodeVms.Remove(item);
                        }
                        foreach (var item in value) {
                            var vm = _wsServerNodeVms.FirstOrDefault(a => a.Address == item.Address);
                            if (vm != null) {
                                vm.Update(item);
                            }
                            else {
                                needSort = true;
                                _wsServerNodeVms.Add(new WsServerNodeStateViewModel(item));
                            }
                        }
                    }
                    if (needSort) {
                        _wsServerNodeVms = new ObservableCollection<WsServerNodeStateViewModel>(_wsServerNodeVms.OrderBy(a => a.Description));
                        OnPropertyChanged(nameof(WsServerNodeVms));
                    }
                }
                OnPropertyChanged(nameof(MinerClientWsSessionCount));
                OnPropertyChanged(nameof(MinerClientSessionCount));
                OnPropertyChanged(nameof(MinerStudioWsSessionCount));
                OnPropertyChanged(nameof(MinerStudioSessionCount));
            }
        }

        public ObservableCollection<WsServerNodeStateViewModel> WsServerNodeVms {
            get {
                return _wsServerNodeVms;
            }
        }

        public WsServerNodeStateViewModel SelectedItemVm {
            get => _selectedItemVm;
            set {
                _selectedItemVm = value;
                OnPropertyChanged(nameof(SelectedItemVm));
            }
        }

        public int MinerClientWsSessionCount {
            get {
                return WsServerNodeVms.Sum(a => a.MinerClientWsSessionCount);
            }
        }

        public int MinerClientSessionCount {
            get {
                return WsServerNodeVms.Sum(a => a.MinerClientSessionCount);
            }
        }

        public int MinerStudioWsSessionCount {
            get {
                return WsServerNodeVms.Sum(a => a.MinerStudioWsSessionCount);
            }
        }

        public int MinerStudioSessionCount {
            get {
                return WsServerNodeVms.Sum(a => a.MinerStudioSessionCount);
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

        public double ProcessMemoryMb {
            get => _processMemoryMb;
            set {
                if (_processMemoryMb != value) {
                    _processMemoryMb = value;
                    OnPropertyChanged(nameof(ProcessMemoryMb));
                    OnPropertyChanged(nameof(ProcessMemoryMbText));
                }
            }
        }

        public string ProcessMemoryMbText {
            get {
                return this.ProcessMemoryMb.ToString("f1") + " Mb";
            }
        }

        public long ThreadCount {
            get {
                return _threadCount;
            }
            set {
                if (_threadCount != value) {
                    _threadCount = value;
                    OnPropertyChanged(nameof(ThreadCount));
                }
            }
        }

        public long HandleCount {
            get {
                return _handleCount;
            }
            set {
                if (_handleCount != value) {
                    _handleCount = value;
                    OnPropertyChanged(nameof(HandleCount));
                }
            }
        }

        public string AvailableFreeSpaceInfo {
            get {
                return _availableFreeSpaceInfo;
            }
            set {
                if (_availableFreeSpaceInfo != value) {
                    _availableFreeSpaceInfo = value;
                    OnPropertyChanged(nameof(AvailableFreeSpaceInfo));
                }
            }
        }

        public int CaptchaCount {
            get { return _captchaCount; }
            set {
                if (_captchaCount != value) {
                    _captchaCount = value;
                    OnPropertyChanged(nameof(CaptchaCount));
                }
            }
        }
    }
}
