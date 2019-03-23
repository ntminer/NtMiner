using NTMiner.Core;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolViewModel : ViewModelBase, IPool, IEditableViewModel {
        public static readonly PoolViewModel Empty = new PoolViewModel(Guid.Empty) {
            _coinId = Guid.Empty,
            _name = "无"
        };
        public static readonly PoolViewModel PleaseSelect = new PoolViewModel(Guid.Empty) {
            _coinId = Guid.Empty,
            _name = "不指定"
        };
        private DataLevel _dataLevel = DataLevel.UnDefined;
        private Guid _id;
        private string _name;
        private Guid _coinId;
        private string _server;
        private string _url;
        private int _sortNumber;
        private string _description;
        private string _userName;
        private string _passWord;
        private bool _isUserMode;
        private PublishStatus _publishState;
        private CoinViewModel _coinVm;


        public Guid GetId() {
            return this.Id;
        }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand SortUp { get; private set; }
        public ICommand SortDown { get; private set; }

        public ICommand ViewPoolIncome { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public PoolViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public PoolViewModel(IPool data) : this(data.GetId()) {
            _dataLevel = data.DataLevel;
            _name = data.Name;
            _coinId = data.CoinId;
            _server = data.Server;
            _url = data.Url;
            _sortNumber = data.SortNumber;
            _description = data.Description;
            _publishState = data.PublishState;
            _userName = data.UserName;
            _passWord = data.Password;
            _isUserMode = data.IsUserMode;
        }

        public PoolViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Current.PoolSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdatePoolCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddPoolCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                PoolEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Name}矿池吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemovePoolCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            });
            this.SortUp = new DelegateCommand(() => {
                PoolViewModel upOne = PoolViewModels.Current.AllPools.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.CoinId == this.CoinId && a.SortNumber < this.SortNumber);
                if (upOne != null) {
                    int sortNumber = upOne.SortNumber;
                    upOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(upOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(this));
                    if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(coinVm.Pools));
                        coinVm.OnPropertyChanged(nameof(coinVm.OptionPools));
                    }
                }
            });
            this.SortDown = new DelegateCommand(() => {
                PoolViewModel nextOne = PoolViewModels.Current.AllPools.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.CoinId == this.CoinId && a.SortNumber > this.SortNumber);
                if (nextOne != null) {
                    int sortNumber = nextOne.SortNumber;
                    nextOne.SortNumber = this.SortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(nextOne));
                    this.SortNumber = sortNumber;
                    VirtualRoot.Execute(new UpdatePoolCommand(this));
                    if (CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(coinVm.Pools));
                        coinVm.OnPropertyChanged(nameof(coinVm.OptionPools));
                    }
                }
            });
            this.ViewPoolIncome = new DelegateCommand<WalletViewModel>((wallet) => {
                if (!string.IsNullOrEmpty(this.Url)) {
                    string url = this.Url;
                    if (this.IsUserMode) {
                        url = url.Replace("{userName}", this.PoolProfileVm.UserName);
                    }
                    else {
                        url = url.Replace("{wallet}", wallet.Address);
                    }
                    url = url.Replace("{worker}", NTMinerRoot.Current.MinerProfile.MinerName);
                    Process.Start(url);
                }
            });
        }

        public bool IsNew {
            get {
                return !NTMinerRoot.Current.PoolSet.Contains(this.Id);
            }
        }

        public List<PoolKernelViewModel> PoolKernels {
            get {
                return PoolKernelViewModels.Current.AllPoolKernels.Where(a => a.PoolId == this.Id).OrderBy(a => a.Kernel.Code + a.Kernel.Version).ToList();
            }
        }

        public DataLevel DataLevel {
            get { return _dataLevel; }
            set {
                if (_dataLevel != value) {
                    _dataLevel = value;
                    OnPropertyChanged(nameof(DataLevel));
                    OnPropertyChanged(nameof(DataLevelText));
                    OnPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        public bool IsReadOnly {
            get {
                if (!DevMode.IsDebugMode && this.DataLevel == DataLevel.Global) {
                    return true;
                }
                return false;
            }
        }

        public string DataLevelText {
            get {
                return this.DataLevel.GetDescription();
            }
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (this == PleaseSelect) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (PoolViewModels.Current.AllPools.Any(a => a.Name == value && a.Id != this.Id && a.CoinId == this.CoinId)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public Guid CoinId {
            get {
                return _coinId;
            }
            set {
                if (_coinId != value) {
                    _coinId = value;
                    OnPropertyChanged(nameof(CoinId));
                    OnPropertyChanged(nameof(CoinVm));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (_coinVm == null || _coinVm.Id != this.CoinId) {
                    CoinViewModels.Current.TryGetCoinVm(this.CoinId, out _coinVm);
                    if (_coinVm == null) {
                        _coinVm = CoinViewModel.PleaseSelect;
                    }
                }
                return _coinVm;
            }
        }

        public string CoinCode {
            get {
                if (NTMinerRoot.Current.CoinSet.TryGetCoin(this.CoinId, out ICoin coin)) {
                    return coin.Code;
                }
                return string.Empty;
            }
        }

        public string Server {
            get => _server;
            set {
                if (_server != value) {
                    _server = value;
                    OnPropertyChanged(nameof(Server));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("矿池地址是必须的");
                    }
                }
            }
        }

        public string Url {
            get => _url;
            set {
                if (_url != value) {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        public int SortNumber {
            get => _sortNumber;
            set {
                if (_sortNumber != value) {
                    _sortNumber = value;
                    OnPropertyChanged(nameof(SortNumber));
                }
            }
        }

        public PublishStatus PublishState {
            get => _publishState;
            set {
                if (_publishState != value) {
                    _publishState = value;
                    OnPropertyChanged(nameof(PublishState));
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

        public bool IsUserMode {
            get { return _isUserMode; }
            set {
                if (_isUserMode != value) {
                    _isUserMode = value;
                    OnPropertyChanged(nameof(IsUserMode));
                }
            }
        }

        public string UserName {
            get { return _userName; }
            set {
                if (_userName != value) {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string Password {
            get { return _passWord; }
            set {
                if (_passWord != value) {
                    _passWord = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public PoolProfileViewModel PoolProfileVm {
            get {
                return PoolProfileViewModels.Current.GetOrCreatePoolProfile(this.Id);
            }
        }
    }
}
