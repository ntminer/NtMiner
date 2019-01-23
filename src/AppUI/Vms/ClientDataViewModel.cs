using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.Vms {
    public class ClientDataViewModel : ViewModelBase, IClientData {
        private readonly ClientData _data;
        public ClientDataViewModel(ClientData data) {
            _data = data;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _data.Id;
            set {
                _data.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Guid WorkId {
            get => _data.WorkId;
            set {
                _data.WorkId = value;
                OnPropertyChanged(nameof(WorkId));
                OnPropertyChanged(nameof(SelectedMineWork));
            }
        }

        public MineWorkViewModel SelectedMineWork {
            get {
                if (WorkId == Guid.Empty) {
                    return MineWorkViewModel.FreeMineWork;
                }
                MineWorkViewModel vm;
                if (MineWorkViewModels.Current.TryGetMineWorkVm(WorkId, out vm)) {
                    return vm;
                }
                return MineWorkViewModel.FreeMineWork;
            }
            set {
                if (WorkId != value.Id) {
                    WorkId = value.Id;
                    OnPropertyChanged(nameof(SelectedMineWork));
                    Server.ControlCenterService.UpdateClientAsync(this.Id, nameof(IClientData.WorkId), value.Id, null);
                }
            }
        }

        public string WorkName {
            get {
                if (this.WorkId == Guid.Empty) {
                    return "自由作业";
                }
                IMineWork mineWork;
                if (NTMinerRoot.Current.MineWorkSet.TryGetMineWork(this.WorkId, out mineWork)) {
                    return mineWork.Name;
                }
                return "未知作业";
            }
        }

        public string PublicKey {
            get => _data.PublicKey;
            set {
                _data.PublicKey = value;
                OnPropertyChanged(nameof(PublicKey));
            }
        }

        public string Version {
            get => _data.Version;
            set {
                _data.Version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        public DateTime ModifiedOn {
            get => _data.ModifiedOn;
            set {
                _data.ModifiedOn = value;
                OnPropertyChanged(nameof(ModifiedOn));
                OnPropertyChanged(nameof(ModifiedOnText));
                OnPropertyChanged(nameof(IsNTMinerOnline));
                OnPropertyChanged(nameof(IsMining));
            }
        }

        public bool IsNTMinerOnline {
            get {
                return this.ModifiedOn.AddSeconds(121) >= DateTime.Now;
            }
        }

        public string ModifiedOnText {
            get {
                return this.ModifiedOn.ToString("HH:mm:ss");
            }
        }

        public bool IsMining {
            get {
                if (!IsNTMinerOnline) {
                    return false;
                }
                return _data.IsMining;
            }
            set {
                _data.IsMining = value;
                OnPropertyChanged(nameof(IsMining));
            }
        }

        public string MinerName {
            get => _data.MinerName;
            set {
                _data.MinerName = value;
                OnPropertyChanged(nameof(MinerName));
            }
        }

        public Guid GroupId {
            get { return _data.GroupId; }
            set {
                _data.GroupId = value;
                OnPropertyChanged(nameof(GroupId));
                OnPropertyChanged(nameof(SelectedMinerGroup));
            }
        }

        private MinerGroupViewModel _selectedMinerGroup;
        public MinerGroupViewModel SelectedMinerGroup {
            get {
                if (_selectedMinerGroup == null || _selectedMinerGroup.Id != GroupId) {
                    MinerGroupViewModels.Current.TryGetMineWorkVm(GroupId, out _selectedMinerGroup);
                    if (_selectedMinerGroup == null) {
                        _selectedMinerGroup = MinerGroupViewModel.PleaseSelect;
                    }
                }
                return _selectedMinerGroup;
            }
            set {
                _selectedMinerGroup = value;
                OnPropertyChanged(nameof(SelectedMinerGroup));
            }
        }

        private MinerGroupViewModel _selectedMinerGroupCopy;
        public MinerGroupViewModel SelectedMinerGroupCopy {
            get {
                if (_selectedMinerGroupCopy == null) {
                    MinerGroupViewModels.Current.TryGetMineWorkVm(GroupId, out _selectedMinerGroupCopy);
                    if (_selectedMinerGroupCopy == null) {
                        _selectedMinerGroupCopy = MinerGroupViewModel.PleaseSelect;
                    }
                }
                return _selectedMinerGroupCopy;
            }
            set {
                _selectedMinerGroupCopy = value;
                OnPropertyChanged(nameof(SelectedMinerGroupCopy));
            }
        }

        private string _minerNameCopy;
        public string MinerNameCopy {
            get => _minerNameCopy;
            set {
                _minerNameCopy = value;
                OnPropertyChanged(nameof(MinerNameCopy));
            }
        }

        public string MinerIp {
            get => _data.MinerIp;
            set {
                _data.MinerIp = value;
                OnPropertyChanged(nameof(MinerIp));
            }
        }

        public string MainCoinCode {
            get => _data.MainCoinCode ?? string.Empty;
            set {
                _data.MainCoinCode = value;
                OnPropertyChanged(nameof(MainCoinCode));
            }
        }

        public CoinViewModel MainCoinVm {
            get {
                CoinViewModel coinVm;
                CoinViewModels.Current.TryGetCoinVm(this.MainCoinCode, out coinVm);
                return coinVm;
            }
        }

        public string MainCoinPool {
            get => _data.MainCoinPool;
            set {
                _data.MainCoinPool = value;
                OnPropertyChanged(nameof(MainCoinPool));
            }
        }

        public string MainCoinWallet {
            get => _data.MainCoinWallet;
            set {
                _data.MainCoinWallet = value;
                OnPropertyChanged(nameof(MainCoinWallet));
            }
        }

        public string Kernel {
            get => _data.Kernel;
            set {
                _data.Kernel = value;
                OnPropertyChanged(nameof(Kernel));
            }
        }

        public bool IsDualCoinEnabled {
            get => _data.IsDualCoinEnabled;
            set {
                _data.IsDualCoinEnabled = value;
                OnPropertyChanged(nameof(IsDualCoinEnabled));
            }
        }

        public string DualCoinCode {
            get => _data.DualCoinCode ?? string.Empty;
            set {
                _data.DualCoinCode = value;
                OnPropertyChanged(nameof(DualCoinCode));
            }
        }

        public CoinViewModel DualCoinVm {
            get {
                CoinViewModel coinVm;
                CoinViewModels.Current.TryGetCoinVm(this.DualCoinCode, out coinVm);
                return coinVm;
            }
        }

        public string DualCoinPool {
            get => _data.DualCoinPool;
            set {
                _data.DualCoinPool = value;
                OnPropertyChanged(nameof(DualCoinPool));
            }
        }

        public string DualCoinWallet {
            get => _data.DualCoinWallet;
            set {
                _data.DualCoinWallet = value;
                OnPropertyChanged(nameof(DualCoinWallet));
            }
        }

        public string GpuInfo {
            get => _data.GpuInfo;
            set {
                _data.GpuInfo = value;
                OnPropertyChanged(nameof(GpuInfo));
            }
        }
    }
}
