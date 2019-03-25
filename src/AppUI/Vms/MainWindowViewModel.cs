using NTMiner.Core;
using NTMiner.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        public static readonly MainWindowViewModel Current = new MainWindowViewModel();

        private Visibility _isBtnRunAsAdministratorVisible = Visibility.Collapsed;
        private string _serverJsonVersion;

        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }
        public ICommand UseThisPcName { get; private set; }

        private MainWindowViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.StartMine = new DelegateCommand(() => {
                this.StateBarVm.IsMining = true;
                NTMinerRoot.Current.StartMine();
            });
            this.StopMine = new DelegateCommand(() => {
                NTMinerRoot.Current.StopMineAsync();
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.GetThisPcName();
                DialogWindow.ShowDialog(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }, icon: IconConst.IconConfirm);
            });
            if (DevMode.IsDevMode) {
                VirtualRoot.On<ServerJsonVersionChangedEvent>("在开发者调试区展示ServerJsonVersion", LogEnum.DevConsole,
                    action: message => {
                        this.ServerJsonVersion = NTMinerRoot.JsonFileVersion;
                    });
                _serverJsonVersion = NTMinerRoot.JsonFileVersion;
            }
        }

        public string BrandTitle {
            get {
                if (NTMinerRoot.KernelBrandId == Guid.Empty) {
                    return string.Empty;
                }
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem(NTMinerRoot.KernelBrandId, out dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                return string.Empty;
            }
        }

        public bool IsUseDevConsole {
            get { return NTMinerRoot.IsUseDevConsole; }
            set {
                NTMinerRoot.IsUseDevConsole = value;
                OnPropertyChanged(nameof(IsUseDevConsole));
            }
        }

        public double Height {
            get {
                return AppStatic.MainWindowHeight;
            }
        }

        public double Width {
            get {
                return AppStatic.MainWindowWidth;
            }
        }

        public Visibility IsBtnRunAsAdministratorVisible {
            get => _isBtnRunAsAdministratorVisible;
            set {
                if (_isBtnRunAsAdministratorVisible != value) {
                    _isBtnRunAsAdministratorVisible = value;
                    OnPropertyChanged(nameof(IsBtnRunAsAdministratorVisible));
                }
            }
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public StateBarViewModel StateBarVm {
            get {
                return StateBarViewModel.Current;
            }
        }

        public List<GpuSpeedViewModel> GpuSpeedVms {
            get { return GpuSpeedViewModels.Current.GpuSpeedVms; }
        }

        public string ServerJsonVersion {
            get => _serverJsonVersion;
            set {
                if (_serverJsonVersion != value) {
                    _serverJsonVersion = value;
                    OnPropertyChanged(nameof(ServerJsonVersion));
                }
            }
        }

        public Visibility IsOverClockVisible {
            get {
                if (Design.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (NTMinerRoot.Current.GpuSet.GpuType == GpuType.NVIDIA) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
