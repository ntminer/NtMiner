using NTMiner.Core.SysDics;
using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public static readonly AboutPageViewModel Current = new AboutPageViewModel();

        private string _imageSource = "/NTMinerWpf;component/Styles/Images/logo128.png";

        private AboutPageViewModel() {
        }

        public string ImageSource {
            get => _imageSource;
            set {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public Version CurrentVersion {
            get {
                return NTMinerRoot.CurrentVersion;
            }
        }

        public int ThisYear {
            get {
                return DateTime.Now.Year;
            }
        }
    }
}
