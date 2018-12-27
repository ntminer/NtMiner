using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelPageViewModel : ViewModelBase {
        public static readonly KernelPageViewModel Current = new KernelPageViewModel();

        private string _keyword;
        private List<KernelMenu> _kernelMenus = new List<KernelMenu>();
        private KernelMenu _currentKernelMenu;
        private Visibility _isBtnUnInstallVisible = Visibility.Collapsed;
        private Visibility _kernelDownloadingVisible = Visibility.Collapsed;

        public ICommand Home { get; private set; }
        public ICommand ChangeCurrentKernelMenu { get; private set; }

        public ICommand Add { get; private set; }
        public ICommand ClearKeyword { get; private set; }

        private readonly KernelMenu _repositoryKernelMenu = new KernelMenu("宝库", "Icon_Kernel");
        private readonly KernelMenu _updateKernelMenu = new KernelMenu("升级", "Icon_Update");
        private readonly KernelMenu _uninstallKernelMenu = new KernelMenu("卸载", "Icon_Delete");

        private KernelPageViewModel() {
            this.ChangeCurrentKernelMenu = new DelegateCommand<KernelMenu>((kernelMenu) => {
                SetCurrentKernelMenu(kernelMenu);
                OnPropertyChanged(nameof(QueryResults));
                KernelDownloadingVisible = Visibility.Collapsed;
            });
            this.Home = new DelegateCommand(() => {
                ChangeCurrentKernelMenu.Execute(_repositoryKernelMenu);
            });

            this._kernelMenus.Add(_repositoryKernelMenu);
            this._kernelMenus.Add(_updateKernelMenu);
            this._kernelMenus.Add(_uninstallKernelMenu);
            this.Add = new DelegateCommand(() => {
                int sortNumber = NTMinerRoot.Current.KernelSet.Count == 0 ? 1 : NTMinerRoot.Current.KernelSet.Max(a => a.SortNumber) + 1;
                new KernelViewModel(Guid.NewGuid()) {
                    SortNumber = sortNumber
                }.Edit.Execute(null);
            });
            this.ClearKeyword = new DelegateCommand(() => {
                Keyword = string.Empty;
            });
            if (!NTMinerRoot.IsInDesignMode) {
                KernelViewModels.Current.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                    if (e.PropertyName == nameof(KernelViewModels.AllKernels)) {
                        OnPropertyChanged(nameof(QueryResults));
                    }
                };
            }
            this.Home.Execute(null);
        }

        public void Download(Guid kernelId, Action<bool, string> downloadComplete) {
            KernelDownloadingVisible = Visibility.Visible;
            KernelViewModel kernelVm;
            if (KernelViewModels.Current.TryGetKernelVm(kernelId, out kernelVm)) {
                kernelVm.KernelProfileVm.Download(downloadComplete);
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        private CoinViewModel _selectedCoinVm = CoinViewModel.PleaseSelect;
        public CoinViewModel SelectedCoinVm {
            get {
                return _selectedCoinVm;
            }
            set {
                _selectedCoinVm = value;
                OnPropertyChanged(nameof(SelectedCoinVm));
                OnPropertyChanged(nameof(QueryResults));
            }
        }

        public Visibility KernelDownloadingVisible {
            get => _kernelDownloadingVisible;
            set {
                _kernelDownloadingVisible = value;
                OnPropertyChanged(nameof(KernelDownloadingVisible));
            }
        }

        public string Keyword {
            get => _keyword;
            set {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
                OnPropertyChanged(nameof(QueryResults));
            }
        }

        public List<KernelViewModel> QueryResults {
            get {
                IQueryable<KernelViewModel> query = KernelViewModels.Current.AllKernels.AsQueryable();
                if (!AppStatic.IsDevMode) {
                    query = query.Where(a => a.PublishState == Core.PublishStatus.Published);
                }
                if (!string.IsNullOrEmpty(Keyword)) {
                    string keyword = this.Keyword.ToLower();
                    query = query.
                        Where(a => (!string.IsNullOrEmpty(a.Code) && a.Code.ToLower().Contains(keyword))
                            || (!string.IsNullOrEmpty(a.Version) && a.Version.ToLower().Contains(keyword))
                            || (!string.IsNullOrEmpty(a.Notice) && a.Notice.ToLower().Contains(keyword)));
                }
                if (SelectedCoinVm != null && SelectedCoinVm != CoinViewModel.PleaseSelect) {
                    query = query.Where(a => a.SupportedCoinVms.Contains(SelectedCoinVm));
                }
                if (CurrentKernelMenu == _updateKernelMenu) {
                    IsBtnUnInstallVisible = Visibility.Collapsed;
                    query = query.Where(a => a.KernelProfileVm.InstallStatus == InstallStatus.CanUpdate);
                }
                else if (CurrentKernelMenu == _uninstallKernelMenu) {
                    IsBtnUnInstallVisible = Visibility.Visible;
                    query = query.Where(a => a.KernelProfileVm.InstallStatus == InstallStatus.CanUpdate || a.KernelProfileVm.InstallStatus == InstallStatus.Installed);
                }
                else {
                    IsBtnUnInstallVisible = Visibility.Collapsed;
                }
                return query.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<KernelViewModel> DownloadingVms {
            get {
                return KernelViewModels.Current.AllKernels.Where(a => a.KernelProfileVm.IsDownloading).OrderBy(a => a.SortNumber).ToList();
            }
        }

        public List<KernelMenu> KernelMenus {
            get { return _kernelMenus; }
            set {
                _kernelMenus = value;
                OnPropertyChanged(nameof(KernelMenus));
            }
        }

        public void SetCurrentKernelMenu(KernelMenu kernelMenu) {
            if (this.CurrentKernelMenu != null) {
                this.CurrentKernelMenu.SetDefaultBackground();
            }
            this.CurrentKernelMenu = kernelMenu;
            if (kernelMenu != null) {
                this.CurrentKernelMenu.SetSelectedBackground();
            }
        }

        public Visibility IsBtnUnInstallVisible {
            get => _isBtnUnInstallVisible;
            set {
                _isBtnUnInstallVisible = value;
                OnPropertyChanged(nameof(IsBtnUnInstallVisible));
            }
        }

        public KernelMenu CurrentKernelMenu {
            get => _currentKernelMenu;
            set {
                _currentKernelMenu = value;
                OnPropertyChanged(nameof(CurrentKernelMenu));
            }
        }
    }
}
