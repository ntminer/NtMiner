using System;
using System.Windows;
using System.Windows.Forms;

namespace NTMiner.Views {
    public class ExtendedNotifyIcon : IDisposable {
        public void Init() {
            // nothing need todo
        }

        private readonly NotifyIcon _targetNotifyIcon;
        public ExtendedNotifyIcon(string iconUrl) {
            _targetNotifyIcon = new NotifyIcon {
                Icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(new Uri(iconUrl)).Stream),
                Visible = true,
                Text = NTMinerRoot.Title,
                ContextMenu = new ContextMenu()
            };
            _targetNotifyIcon.ContextMenu.MenuItems.Add(new MenuItem("退出", (sender, e) => {
                System.Windows.Application.Current.MainWindow.Close();
            }));
            _targetNotifyIcon.MouseDown += (object sender, MouseEventArgs e)=> {
                if (e.Button == MouseButtons.Left) {
                    ToggleWindow();
                }
            };
            DateTime t = DateTime.Now;
            _targetNotifyIcon.MouseMove += (object sender, MouseEventArgs e)=> {
                if (t.AddSeconds(2) < DateTime.Now) {
                    t = DateTime.Now;
                    // TODO:显示基本信息
                    _targetNotifyIcon.Text = NTMinerRoot.Title;
                }
            };
        }

        public void ToggleWindow() {
            Window mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow == null) {
                return;
            }
            if (mainWindow.WindowState == WindowState.Minimized) {
                mainWindow.WindowState = WindowState.Normal;
            }
            else {
                mainWindow.WindowState = WindowState.Minimized;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Standard IDisposable interface implementation. If you dont dispose the windows notify icon, the application
        /// closes but the icon remains in the task bar until such time as you mouse over it.
        /// </summary>
        private bool _IsDisposed = false;

        ~ExtendedNotifyIcon() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            // Tell the garbage collector not to call the finalizer
            // since all the cleanup will already be done.
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool IsDisposing) {
            if (_IsDisposed)
                return;

            if (IsDisposing) {
                _targetNotifyIcon.Dispose();
            }

            // Free any unmanaged resources in this section
            _IsDisposed = true;
        }

        #endregion
    }
}
