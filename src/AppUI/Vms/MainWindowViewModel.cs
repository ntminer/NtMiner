using NTMiner.Core;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        public static readonly MainWindowViewModel Current = new MainWindowViewModel();

        private Visibility _isBtnRunAsAdministratorVisible = Visibility.Collapsed;
        private string _serverJsonVersion;

        public ICommand CustomTheme { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }
        public ICommand UseThisPcName { get; private set; }

        private MainWindowViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.CustomTheme = new DelegateCommand(() => {
                LogColor.ShowWindow();
            });
            this.StartMine = new DelegateCommand(() => {
                this.StateBarVm.IsMining = true;
                NTMinerRoot.Current.StartMine();
            });
            this.StopMine = new DelegateCommand(() => {
                if (!NTMinerRoot.Current.IsMining) {
                    this.StateBarVm.IsMining = false;
                }
                NTMinerRoot.Current.StopMineAsync();
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.GetThisPcName();
                DialogWindow.ShowDialog(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }, icon: IconConst.IconConfirm);
            });
            VirtualRoot.On<StartingMineFailedEvent>("开始挖矿失败", LogEnum.DevConsole,
                action: message => {
                    this.StateBarVm.IsMining = false;
                    Write.UserFail(message.Message);
                });
            if (DevMode.IsDevMode) {
                VirtualRoot.On<ServerJsonVersionChangedEvent>("开发者模式展示ServerJsonVersion", LogEnum.DevConsole,
                    action: message => {
                        this.ServerJsonVersion = GetServerJsonVersion();
                    });
                _serverJsonVersion = GetServerJsonVersion();
            }
        }

        private string GetServerJsonVersion() {
            string serverJsonVersion = string.Empty;
            if (NTMinerRoot.Current.LocalAppSettingSet.TryGetAppSetting("ServerJsonVersion", out IAppSetting setting) && setting.Value != null) {
                serverJsonVersion = setting.Value.ToString();
            }
            return serverJsonVersion;
        }

        public string BrandTitle {
            get {
                if (VirtualRoot.KernelBrandId == Guid.Empty) {
                    return string.Empty;
                }
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem(VirtualRoot.KernelBrandId, out ISysDicItem dicItem)) {
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
