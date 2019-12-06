using System;
using System.Drawing;
using System.Windows.Forms;

namespace NTMiner {
    public class ExtendedNotifyIcon : IDisposable {
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
        }

        public void RefreshIcon() {
            _targetNotifyIcon.Visible = _isMinerStudio || NTMinerRoot.Instance.MinerProfile.IsShowNotifyIcon;
        }

        #region IDisposable Members

        /// <summary>
        /// Standard IDisposable interface implementation. If you dont dispose the windows notify icon, the application
        /// closes but the icon remains in the task bar until such time as you mouse over it.
        /// </summary>
        private bool _isDisposed = false;

        ~ExtendedNotifyIcon() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            // Tell the garbage collector not to call the finalizer
            // since all the cleanup will already be done.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing) {
            if (_isDisposed)
                return;

            if (isDisposing) {
                _targetNotifyIcon.Dispose();
            }

            // Free any unmanaged resources in this section
            _isDisposed = true;
        }

        #endregion
    }
}
