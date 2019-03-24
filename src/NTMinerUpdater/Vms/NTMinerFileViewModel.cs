using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class NTMinerFileViewModel : ViewModelBase, INTMinerFile, IEditableViewModel {
        private Version _versionData;
        private string _fileName;
        private string _version;
        private string _versionTag;
        private DateTime _createdOn;
        private Guid _id;
        private NTMinerAppType _appType;
        private DateTime _publishOn;

        public ICommand Save { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Remove { get; private set; }

        public NTMinerFileViewModel() {
            _id = Guid.NewGuid();
            _publishOn = DateTime.Now;
            this.Save = new DelegateCommand(() => {
                if (Login()) {
                    OfficialServer.FileUrlService.AddOrUpdateNTMinerFileAsync(new NTMinerFileData().Update(this), (response, e) => {
                        if (response.IsSuccess()) {
                            MainWindowViewModel.Current.Refresh();
                            UIThread.Execute(() => {
                                TopWindow.GetTopWindow()?.Close();
                            });
                        }
                        else {
                            Logger.ErrorDebugLine($"AddOrUpdateNTMinerFileAsync失败");
                        }
                    });
                }
            });
            this.Edit = new DelegateCommand(() => {
                NTMinerFileEdit window = new NTMinerFileEdit("编辑", "Icon_Edit", new NTMinerFileViewModel(this));
                window.ShowDialogEx();
            });
            this.Remove = new DelegateCommand(() => {
                if (Login()) {
                    DialogWindow.ShowDialog(message: $"确定删除{this.Version}({this.VersionTag})吗？", title: "确认", onYes: () => {
                        OfficialServer.FileUrlService.RemoveNTMinerFileAsync(this.Id, (response, e) => {
                            MainWindowViewModel.Current.SelectedNTMinerFile = MainWindowViewModel.Current.NTMinerFiles.FirstOrDefault();
                            if (this == MainWindowViewModel.Current.ServerLatestVm) {
                                MainWindowViewModel.Current.ServerLatestVm = MainWindowViewModel.Current.NTMinerFiles
                                    .FirstOrDefault(a => a != this && a.VersionData > MainWindowViewModel.Current.LocalNTMinerVersion);
                            }
                            MainWindowViewModel.Current.Refresh();
                        });
                    }, icon: IconConst.IconConfirm);
                }
            });
        }

        private bool Login() {
            if (string.IsNullOrEmpty(SingleUser.LoginName) || string.IsNullOrEmpty(SingleUser.PasswordSha1)) {
                LoginWindow window = new LoginWindow() {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = TopWindow.GetTopWindow()
                };
                var result = window.ShowDialogEx();
                return result.HasValue && result.Value;
            }
            return true;
        }

        public NTMinerFileViewModel(INTMinerFile data) : this() {
            _id = data.Id;
            _appType = data.AppType;
            _version = data.Version;
            _fileName = data.FileName;
            _versionTag = data.VersionTag;
            _createdOn = data.CreatedOn;
            _publishOn = data.PublishOn;
            if (!System.Version.TryParse(_version, out _versionData)) {
                _versionData = new Version(1, 0);
            }
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public NTMinerAppType AppType {
            get { return _appType; }
            set {
                _appType = value;
                OnPropertyChanged(nameof(AppType));
            }
        }

        public string FileName {
            get => _fileName;
            set {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
                if (MainWindowViewModel.Current.NTMinerFiles.Any(a => string.Equals(a.FileName, value, StringComparison.OrdinalIgnoreCase) && a.Id != this.Id)) {
                    throw new ValidationException("重复的文件名");
                }
            }
        }

        public string Version {
            get => _version;
            set {
                _version = value;
                OnPropertyChanged(nameof(Version));
                if (!System.Version.TryParse(this.Version, out _versionData)) {
                    _versionData = new Version(1, 0);
                }
                OnPropertyChanged(nameof(VersionData));
                if (MainWindowViewModel.Current.NTMinerFiles.Any(a => a.Version == value && a.Id != this.Id)) {
                    throw new ValidationException("重复的版本号");
                }
            }
        }

        public Version VersionData {
            get {
                return _versionData;
            }
        }

        public string VersionTag {
            get => _versionTag;
            set {
                _versionTag = value;
                OnPropertyChanged(nameof(VersionTag));
                if (MainWindowViewModel.Current.NTMinerFiles.Any(a => a.Version == value && a.Id != this.Id)) {
                    throw new ValidationException("重复的别名");
                }
            }
        }

        public DateTime CreatedOn {
            get => _createdOn;
            set {
                _createdOn = value;
                OnPropertyChanged(nameof(CreatedOn));
            }
        }

        public string PublishOnText {
            get {
                return this.PublishOn.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public DateTime PublishOn {
            get => _publishOn;
            set {
                _publishOn = value;
                OnPropertyChanged(nameof(PublishOn));
                OnPropertyChanged(nameof(PublishOnText));
            }
        }
    }
}
