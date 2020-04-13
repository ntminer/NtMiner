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
                Clipboard.SetDataObject(wallet, true);
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
                    NTMinerContext.Instance.MinerProfile.SetPoolProfileProperty(this.PoolId, nameof(UserName), value ?? string.Empty);
                    OnPropertyChanged(nameof(UserName));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("矿池级内核Profile上放置的矿池的挖矿用户名发生了变更");
                }
            }
        }

        public string Password {
            get => _inner.Password;
            set {
                if (_inner.Password != value) {
                    NTMinerContext.Instance.MinerProfile.SetPoolProfileProperty(this.PoolId, nameof(Password), value ?? string.Empty);
                    OnPropertyChanged(nameof(Password));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("矿池级内核Profile上放置的矿池的挖矿密码发生了变更");
                }
            }
        }
    }
}
