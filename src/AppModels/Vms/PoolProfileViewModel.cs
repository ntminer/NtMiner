using NTMiner.Core.Profile;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolProfileViewModel : ViewModelBase, IPoolProfile {
        private readonly IPoolProfile _inner;

        public ICommand CopyWallet { get; private set; }

        public PoolProfileViewModel(IPoolProfile innerProfile) {
            _inner = innerProfile;
            this.CopyWallet = new DelegateCommand(() => {
                string wallet = this.UserName ?? "无";
                Clipboard.SetDataObject(wallet);
                VirtualRoot.Out.ShowSuccess(wallet, header: "复制成功");
            });
        }

        public Guid PoolId {
            get { return _inner.PoolId; }
        }

        public string UserName {
            get => _inner.UserName;
            set {
                if (_inner.UserName != value) {
                    NTMinerRoot.Instance.MinerProfile.SetPoolProfileProperty(this.PoolId, nameof(UserName), value ?? string.Empty);
                    OnPropertyChanged(nameof(UserName));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public string Password {
            get => _inner.Password;
            set {
                if (_inner.Password != value) {
                    NTMinerRoot.Instance.MinerProfile.SetPoolProfileProperty(this.PoolId, nameof(Password), value ?? string.Empty);
                    OnPropertyChanged(nameof(Password));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
    }
}
