using NTMiner.Core.MinerServer;
using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class NTMinerWalletViewModel : ViewModelBase, INTMinerWallet, IEditableViewModel {
        private Guid _id;
        private string _coinCode;
        private string _wallet;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public NTMinerWalletViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public NTMinerWalletViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerContext.MinerStudioContext.NTMinerWalletSet.TryGetNTMinerWallet(Id, out INTMinerWallet _)) {
                    VirtualRoot.Execute(new UpdateNTMinerWalletCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddNTMinerWalletCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditNTMinerWalletCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定删除吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveNTMinerWalletCommand(this.Id));
                }));
            });
        }

        public NTMinerWalletViewModel(INTMinerWallet data) : this(data.GetId()) {
            _coinCode = data.CoinCode;
            _wallet = data.Wallet;
        }

        public Guid GetId() {
            return Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string CoinCode {
            get => _coinCode;
            set {
                _coinCode = value;
                OnPropertyChanged(nameof(CoinCode));
            }
        }

        public string Wallet {
            get => _wallet;
            set {
                _wallet = value;
                OnPropertyChanged(nameof(Wallet));
            }
        }

        public MainMenuViewModel MainMenu {
            get {
                return MainMenuViewModel.Instance;
            }
        }
    }
}
