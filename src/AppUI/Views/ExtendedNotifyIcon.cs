using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using NTMiner.Vms;

namespace NTMiner.Views {
    public class ExtendedNotifyIcon : IDisposable {
        public static ExtendedNotifyIcon Create(Icon icon, string text, bool isMinerStudio) {
            return new ExtendedNotifyIcon(icon, text, isMinerStudio);
        }

        private readonly NotifyIcon _targetNotifyIcon;
        private readonly bool _isMinerStudio;
        private ExtendedNotifyIcon(Icon icon, string text, bool isMinerStudio) {
            _isMinerStudio = isMinerStudio;
            _targetNotifyIcon = new NotifyIcon {
                Icon = icon,
                Visible = isMinerStudio || NTMinerRegistry.GetIsShowNotifyIcon(),
                Text = text,
                ContextMenu = new ContextMenu()
            };
            _targetNotifyIcon.ContextMenu.MenuItems.Add(new MenuItem("退出" + text, (sender, e) => {
                AppStatic.AppExit.Execute(null);
            }));
            _targetNotifyIcon.MouseDown += (object sender, MouseEventArgs e) => {
                if (e.Button == MouseButtons.Left) {
                    ToggleWindow();
                }
            };
        }

        public void RefreshIcon() {
            _targetNotifyIcon.Visible = _isMinerStudio || NTMinerRegistry.GetIsShowNotifyIcon();
        }

        public void ToggleWindow() {
            Window mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow == null) {
                VirtualRoot.Execute(new ShowMainWindowCommand());
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
        private bool _isDisposed = false;

        ~ExtendedNotifyIcon() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            // Tell the garbage collector not to call the finalizer
            // since all the cleanup will already be done.
            GC.SuppressFinalize(true);
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
