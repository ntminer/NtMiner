using System;
using System.Drawing;
using System.Windows.Forms;

namespace NTMiner {
    public class ExtendedNotifyIcon : IDisposable {
        public static ExtendedNotifyIcon Create(Icon icon, string text) {
            return new ExtendedNotifyIcon(icon, text);
        }

        private readonly NotifyIcon _targetNotifyIcon;
        private ExtendedNotifyIcon(Icon icon, string text) {
            _targetNotifyIcon = new NotifyIcon {
                Icon = icon,
                Visible = NTMinerRegistry.GetIsShowNotifyIcon(),
                Text = text,
                ContextMenu = new ContextMenu()
            };
        }

        public void ShowIcon() {
            _targetNotifyIcon.Visible = true;
        }

        public void HideIcon() {
            _targetNotifyIcon.Visible = false;
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
