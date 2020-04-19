using NTMiner.Microsoft.Windows.Shell.Standard;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using WM = NTMiner.Models.Win32.WM;

namespace NTMiner.Behaviours {
    public class GlowWindowBehavior : Behavior<Window> {
        private GlowWindow left, right, top, bottom;
        private IntPtr handle;

        protected override void OnAttached() {
            base.OnAttached();

            this.AssociatedObject.SourceInitialized += (o, args) => {
                handle = new WindowInteropHelper(this.AssociatedObject).Handle;
                var hwndSource = HwndSource.FromHwnd(handle);
                if (hwndSource != null) {
                    hwndSource.AddHook(AssociatedObjectWindowProc);
                }
            };
            this.AssociatedObject.Loaded += (sender, e)=> {
                this.left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
                this.right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
                this.top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
                this.bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);

                left.Show();
                left.Update();

                right.Show();
                right.Update();

                top.Show();
                top.Update();

                bottom.Show();
                bottom.Update();
            };
        }

        private WINDOWPOS _previousWP;

        private IntPtr AssociatedObjectWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch ((WM)msg) {
                case WM.WINDOWPOSCHANGED:
                case WM.WINDOWPOSCHANGING:
                    Assert.IsNotDefault(lParam);
                    var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    if (!wp.Equals(_previousWP)) {
                        this.UpdateCore();
                    }
                    _previousWP = wp;
                    break;
                case WM.SIZE:
                case WM.SIZING:
                    this.UpdateCore();
                    break;
            }
            return IntPtr.Zero;
        }

        private void UpdateCore() {
            if (handle != IntPtr.Zero && Native.UnsafeNativeMethods.GetWindowRect(handle, out Native.RECT rect)) {
                if (left != null) left.UpdateCore(rect);
                if (right != null) right.UpdateCore(rect);
                if (top != null) top.UpdateCore(rect);
                if (bottom != null) bottom.UpdateCore(rect);
            }
        }
    }
}
