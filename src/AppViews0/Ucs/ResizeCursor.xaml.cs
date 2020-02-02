using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace NTMiner.Views.Ucs {
    public partial class ResizeCursor : UserControl {
        #region SafeNativeMethods
        private static class SafeNativeMethods {
            #region enum struct class
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public class MONITORINFO {
                public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                public RECT rcMonitor = new RECT();
                public RECT rcWork = new RECT();
                public int dwFlags = 0;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT {
                public int Left, Top, Right, Bottom;
            }

            public enum ResizeDirection {
                Left = 1,
                Right = 2,
                Top = 3,
                TopLeft = 4,
                TopRight = 5,
                Bottom = 6,
                BottomLeft = 7,
                BottomRight = 8,
            }
            #endregion

            [DllImport(DllName.User32Dll, CharSet = CharSet.Auto)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }
        #endregion

        private HwndSource hwndSource;
        public ResizeCursor() {
            this.Loaded += (sender, e) => {
                Window window = Window.GetWindow(this);
                hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            };
            InitializeComponent();
        }

        private void Resize(object sender, MouseButtonEventArgs e) {
            this.ResizeWindow(sender);
        }

        private void DisplayResizeCursor(object sender, MouseEventArgs e) {
            this.DisplayResizeCursor(sender);
        }

        private void ResetCursor(object sender, MouseEventArgs e) {
            if (Mouse.LeftButton != MouseButtonState.Pressed) {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void ResizeWindow(SafeNativeMethods.ResizeDirection direction) {
            const int WM_SYSCOMMAND = 0x112;
            SafeNativeMethods.SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private void ResizeWindow(object sender) {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name) {
                case nameof(this.top):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Top);
                    break;
                case nameof(this.bottom):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Bottom);
                    break;
                case nameof(this.left):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Left);
                    break;
                case nameof(this.right):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.Right);
                    break;
                case nameof(this.topLeft):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.TopLeft);
                    break;
                case nameof(this.topRight):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.TopRight);
                    break;
                case nameof(this.bottomLeft):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.BottomLeft);
                    break;
                case nameof(this.bottomRight):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(SafeNativeMethods.ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void DisplayResizeCursor(object sender) {
            Rectangle clickedRectangle = sender as Rectangle;

            switch (clickedRectangle.Name) {
                case nameof(this.top):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    break;
                case nameof(this.bottom):
                    clickedRectangle.Cursor = Cursors.SizeNS;
                    break;
                case nameof(this.left):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    break;
                case nameof(this.right):
                    clickedRectangle.Cursor = Cursors.SizeWE;
                    break;
                case nameof(this.topLeft):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    break;
                case nameof(this.topRight):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    break;
                case nameof(this.bottomLeft):
                    clickedRectangle.Cursor = Cursors.SizeNESW;
                    break;
                case nameof(this.bottomRight):
                    clickedRectangle.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }
    }
}
