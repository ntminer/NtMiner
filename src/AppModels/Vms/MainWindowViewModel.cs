using NTMiner.Core;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private readonly StateBarViewModel _stateBarVm = new StateBarViewModel();

        public ICommand UseThisPcName { get; private set; }
        public ICommand CloseMainWindow { get; private set; }

        public MainWindowViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.CloseMainWindow = new DelegateCommand(() => {
                VirtualRoot.Execute(new CloseMainWindowCommand());
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.ThisPcName;
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }));
            });
        }

        public bool IsTestHost {
            get {
                return !string.IsNullOrEmpty(Hosts.GetIp(NTKeyword.ServerHost, out long _));
            }
            set {
                if (value) {
                    Hosts.SetHost(NTKeyword.ServerHost, "127.0.0.1");
                }
                else {
                    Hosts.SetHost(NTKeyword.ServerHost, string.Empty);
                }
                OnPropertyChanged(nameof(IsTestHost));
            }
        }

        public string BrandTitle {
            get {
                if (NTMinerRoot.KernelBrandId == Guid.Empty && NTMinerRoot.PoolBrandId == Guid.Empty) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTMinerRoot.KernelBrandId, out ISysDicItem dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                else if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTMinerRoot.PoolBrandId, out dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                return string.Empty;
            }
        }

        public StateBarViewModel StateBarVm {
            get => _stateBarVm;
        }

        public bool IsUseDevConsole {
            get { return NTMinerRoot.IsUseDevConsole; }
            set {
                NTMinerRoot.IsUseDevConsole = value;
                OnPropertyChanged(nameof(IsUseDevConsole));
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }
    }
}
