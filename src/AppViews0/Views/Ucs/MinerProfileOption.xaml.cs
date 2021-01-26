using NTMiner.Vms;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class MinerProfileOption : UserControl {
        public MinerProfileViewModel Vm { get; private set; }

        private readonly Brush _outerUserGroupBg;
        private readonly Brush _automationGroupBg;
        public MinerProfileOption() {
            this.Vm = AppRoot.MinerProfileVm;
            this.DataContext = AppRoot.MinerProfileVm;
            InitializeComponent();
            _outerUserGroupBg = OuterUserGroup.BorderBrush;
            _automationGroupBg = AutomationGroup.BorderBrush;
            this.OnLoaded(window => {
                VirtualRoot.BuildEventPath<SignUpedEvent>("注册了新外网群控用户后自动填入外网群控用户名", LogEnum.None, path: message => {
                    this.Vm.OuterUserId = message.LoginName;
                }, this.GetType());
            });
        }

        private void ButtonHotKey_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z) {
                Vm.HotKey = e.Key.ToString();
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        // 高亮外网群控区域
        public void HighlightOuterUser() {
            OuterUserGroup.BorderBrush = WpfUtil.RedBrush;
            OuterUserGroup.BringIntoView();
            TimeSpan.FromSeconds(1).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    OuterUserGroup.BorderBrush = _outerUserGroupBg;
                });
            });
        }

        // 高亮自动化区域
        public void HighlightAutomation() {
            AutomationGroup.BorderBrush = WpfUtil.RedBrush;
            AutomationGroup.BringIntoView();
            TimeSpan.FromSeconds(1).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    AutomationGroup.BorderBrush = _automationGroupBg;
                });
            });
        }

        private void TbWsDaemonStateDescription_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Vm.RefreshWsDaemonState();
        }
    }
}
