using NTMiner.Core.MinerServer;
using NTMiner.Vms;
using System;

namespace NTMiner.MinerStudio.Vms {
    public class NTMinerFileViewModel : ViewModelBase, INTMinerFile {
        public static readonly NTMinerFileViewModel Empty = new NTMinerFileViewModel(new NTMinerFileData {
            AppType = NTMinerAppType.MinerClient,
            CreatedOn = Timestamp.UnixBaseTime,
            Description = string.Empty,
            FileName = string.Empty,
            Id = Guid.Empty,
            PublishOn = Timestamp.UnixBaseTime,
            Title = string.Empty,
            Version = "1.0.0",
            VersionTag = string.Empty
        });

        private Version _versionData;
        private string _fileName;
        private string _version;
        private string _versionTag;
        private DateTime _createdOn;
        private Guid _id;
        private NTMinerAppType _appType;
        private DateTime _publishOn;
        private string _title;
        private string _description;

        public NTMinerFileViewModel(INTMinerFile data) {
            _id = data.Id;
            _appType = data.AppType;
            _version = data.Version;
            _fileName = data.FileName;
            _versionTag = data.VersionTag;
            _createdOn = data.CreatedOn;
            _publishOn = data.PublishOn;
            _title = data.Title;
            _description = data.Description;
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
            }
        }

        public string Title {
            get => _title;
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
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
                return this.PublishOn.ToString("yyyy-MM-dd");
            }
        }

        public DateTime PublishOn {
            get => _publishOn;
            set {
                _publishOn = value;
                OnPropertyChanged(nameof(PublishOn));
            }
        }
    }
}
