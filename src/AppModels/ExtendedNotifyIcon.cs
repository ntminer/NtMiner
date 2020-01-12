using System;
using System.Drawing;
using System.Windows.Forms;

namespace NTMiner {
    public class ExtendedNotifyIcon {
        public static ExtendedNotifyIcon Create(string text, bool isMinerStudio) {
            string url;
            if (isMinerStudio) {
                url = "pack://application:,,,/MinerStudio;component/logo.ico";
            }
            else {
                url = "pack://application:,,,/NTMiner;component/logo.ico";
            }
            Icon icon = new Icon(System.Windows.Application.GetResourceStream(new Uri(url)).Stream);
            return new ExtendedNotifyIcon(icon, text, isMinerStudio);
        }

        private readonly NotifyIcon _targetNotifyIcon;
        private readonly bool _isMinerStudio;
        private ExtendedNotifyIcon(Icon icon, string text, bool isMinerStudio) {
            _isMinerStudio = isMinerStudio;
            _targetNotifyIcon = new NotifyIcon {
                Icon = icon,
                Visible = isMinerStudio || NTMinerRoot.Instance.MinerProfile.IsShowNotifyIcon,
                Text = text,
                ContextMenuStrip = new ContextMenuStrip {
                    BackColor = Color.White,
                    ShowImageMargin = false
                }
            };
            _targetNotifyIcon.ContextMenuStrip.Items.Add("退出" + text, null, (sender, e)=> {
                AppStatic.AppExit.Execute(null);
            });
            _targetNotifyIcon.MouseDown += (object sender, MouseEventArgs e) => {
                if (e.Button == MouseButtons.Left) {
                    VirtualRoot.Execute(new ShowMainWindowCommand(isToggle: true));
                }
            };
            VirtualRoot.AddEventPath<AppExitEvent>("退出托盘图标", LogEnum.None, action: message => {
                _targetNotifyIcon.Dispose();
            }, typeof(ExtendedNotifyIcon));
        }

        public void RefreshIcon() {
            _targetNotifyIcon.Visible = _isMinerStudio || NTMinerRoot.Instance.MinerProfile.IsShowNotifyIcon;
        }
    }
}
