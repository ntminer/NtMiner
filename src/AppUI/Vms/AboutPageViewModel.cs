using NTMiner.Core;
using NTMiner.Core.SysDics;
using System;

namespace NTMiner.Vms {
    public class AboutPageViewModel : ViewModelBase {
        public static readonly AboutPageViewModel Current = new AboutPageViewModel();

        private ISysDicItem dicItem;
        private string _imageSource = "/NTMinerWpf;component/Styles/Images/logo128.png";

        private AboutPageViewModel() {
            if (!NTMinerRoot.IsInDesignMode) {
                Global.Access<SysDicItemUpdatedEvent>(
                    Guid.Parse("ec86ee32-f988-4117-abb2-c0d4388b201f"),
                    "更新了系统字典项AboutText后调整VM内存",
                    LogEnum.Log,
                    action: message => {
                        if (dicItem == null || message.Source.GetId() == dicItem.GetId()) {
                            OnPropertyChanged(nameof(AboutText));
                        }
                    });
            }
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

        public string NTMinerTitle {
            get {
                return NTMinerRoot.Title;
            }
        }

        public int ThisYear {
            get {
                return DateTime.Now.Year;
            }
        }

        public string AboutText {
            get {
                if (!NTMinerRoot.IsInDesignMode && NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("ThisSystem", "AboutText", out dicItem)) {
                    return dicItem.Value;
                }
                return "做好用的集成挖矿工具";
            }
        }
    }
}
