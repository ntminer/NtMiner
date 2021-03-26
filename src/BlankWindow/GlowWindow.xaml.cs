using NTMiner.Models.Win32;
using NTMiner.Native;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using RECT = NTMiner.Native.RECT;

namespace NTMiner {
    partial class GlowWindow : Window, IDisposable {
        private readonly Func<Point, Cursor> _getCursor;
        private readonly Func<Point, HitTestValues> _getHitTestValue;
        private readonly Func<RECT, double> _getLeft;
        private readonly Func<RECT, double> _getTop;
        private readonly Func<RECT, double> _getWidth;
        private readonly Func<RECT, double> _getHeight;
        private const double _edgeSize = 20.0;
        private const double _glowSize = 6.0;
        private IntPtr _handle;
        private IntPtr _ownerHandle;
        private bool _closing = false;
        private HwndSource _hwndSource;
        private PropertyChangeNotifier _resizeModeChangeNotifier;

        public GlowWindow(Window owner, GlowDirection direction) {
            InitializeComponent();

            this.AllowsTransparency = true;
            this.Closing += (sender, e) => e.Cancel = !_closing;

            this.Owner = owner;

            this.Owner.StateChanged += (sender, args) => {
                if (this.Owner.WindowState == WindowState.Normal) {
                    this.Show();
                }
                else {
                    this.Hide();
                }
            };

            var b = new Binding("GlowBrush") {
                Source = owner
            };
            glow.SetBinding(Glow.GlowBrushProperty, b);

            b = new Binding("BorderThickness") {
                Source = owner
            };
            glow.SetBinding(BorderThicknessProperty, b);

            glow.Direction = direction;

            switch (direction) {
                case GlowDirection.Left:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Right;
                    _getLeft = (rect) => rect.left - _glowSize + 1;
                    _getTop = (rect) => rect.top - 2;
                    _getWidth = (rect) => _glowSize;
                    _getHeight = (rect) => rect.Height + 4;
                    _getHitTestValue = p => new Rect(0, 0, ActualWidth, _edgeSize).Contains(p)
                                               ? HitTestValues.HTTOPLEFT
                                               : new Rect(0, ActualHeight - _edgeSize, ActualWidth, _edgeSize).Contains(p)
                                                     ? HitTestValues.HTBOTTOMLEFT
                                                     : HitTestValues.HTLEFT;
                    _getCursor = p => {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, ActualWidth, _edgeSize).Contains(p)
                                         ? Cursors.SizeNWSE
                                         : new Rect(0, ActualHeight - _edgeSize, ActualWidth, _edgeSize).Contains(p)
                                               ? Cursors.SizeNESW
                                               : Cursors.SizeWE;
                    };
                    break;
                case GlowDirection.Right:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Left;
                    _getLeft = (rect) => rect.right - 1;
                    _getTop = (rect) => rect.top - 2;
                    _getWidth = (rect) => _glowSize;
                    _getHeight = (rect) => rect.Height + 4;
                    _getHitTestValue = p => new Rect(0, 0, ActualWidth, _edgeSize).Contains(p)
                                               ? HitTestValues.HTTOPRIGHT
                                               : new Rect(0, ActualHeight - _edgeSize, ActualWidth, _edgeSize).Contains(p)
                                                     ? HitTestValues.HTBOTTOMRIGHT
                                                     : HitTestValues.HTRIGHT;
                    _getCursor = p => {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, ActualWidth, _edgeSize).Contains(p)
                                         ? Cursors.SizeNESW
                                         : new Rect(0, ActualHeight - _edgeSize, ActualWidth, _edgeSize).Contains(p)
                                               ? Cursors.SizeNWSE
                                               : Cursors.SizeWE;
                    };
                    break;
                case GlowDirection.Top:
                    glow.Orientation = Orientation.Horizontal;
                    glow.VerticalAlignment = VerticalAlignment.Bottom;
                    _getLeft = (rect) => rect.left - 2;
                    _getTop = (rect) => rect.top - _glowSize + 1;
                    _getWidth = (rect) => rect.Width + 4;
                    _getHeight = (rect) => _glowSize;
                    _getHitTestValue = p => new Rect(0, 0, _edgeSize - _glowSize, ActualHeight).Contains(p)
                                               ? HitTestValues.HTTOPLEFT
                                               : new Rect(Width - _edgeSize + _glowSize, 0, _edgeSize - _glowSize,
                                                          ActualHeight).Contains(p)
                                                     ? HitTestValues.HTTOPRIGHT
                                                     : HitTestValues.HTTOP;
                    _getCursor = p => {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, _edgeSize - _glowSize, ActualHeight).Contains(p)
                                         ? Cursors.SizeNWSE
                                         : new Rect(Width - _edgeSize + _glowSize, 0, _edgeSize - _glowSize, ActualHeight).
                                               Contains(p)
                                               ? Cursors.SizeNESW
                                               : Cursors.SizeNS;
                    };
                    break;
                case GlowDirection.Bottom:
                    glow.Orientation = Orientation.Horizontal;
                    glow.VerticalAlignment = VerticalAlignment.Top;
                    _getLeft = (rect) => rect.left - 2;
                    _getTop = (rect) => rect.bottom - 1;
                    _getWidth = (rect) => rect.Width + 4;
                    _getHeight = (rect) => _glowSize;
                    _getHitTestValue = p => new Rect(0, 0, _edgeSize - _glowSize, ActualHeight).Contains(p)
                                               ? HitTestValues.HTBOTTOMLEFT
                                               : new Rect(Width - _edgeSize + _glowSize, 0, _edgeSize - _glowSize,
                                                          ActualHeight).Contains(p)
                                                     ? HitTestValues.HTBOTTOMRIGHT
                                                     : HitTestValues.HTBOTTOM;
                    _getCursor = p => {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, _edgeSize - _glowSize, ActualHeight).Contains(p)
                                         ? Cursors.SizeNESW
                                         : new Rect(Width - _edgeSize + _glowSize, 0, _edgeSize - _glowSize, ActualHeight).
                                               Contains(p)
                                               ? Cursors.SizeNWSE
                                               : Cursors.SizeNS;
                    };
                    break;
            }

            owner.Closed += (sender, e) => {
                _closing = true;
                Close();
            };
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            this._hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (_hwndSource == null) {
                return;
            }

            var ws = _hwndSource.Handle.GetWindowLong();
            var wsex = _hwndSource.Handle.GetWindowLongEx();

            //ws |= WS.POPUP;
            wsex ^= WSEX.APPWINDOW;
            wsex |= WSEX.NOACTIVATE;
            if (this.Owner.ResizeMode == ResizeMode.NoResize || this.Owner.ResizeMode == ResizeMode.CanMinimize) {
                wsex |= WSEX.TRANSPARENT;
            }

            _hwndSource.Handle.SetWindowLong(ws);
            _hwndSource.Handle.SetWindowLongEx(wsex);
            _hwndSource.AddHook(WndProc);

            _handle = _hwndSource.Handle;
            _ownerHandle = new WindowInteropHelper(Owner).Handle;

            this._resizeModeChangeNotifier = new PropertyChangeNotifier(this.Owner, Window.ResizeModeProperty);
            this._resizeModeChangeNotifier.ValueChanged += ResizeModeChanged;
        }

        private void ResizeModeChanged(object sender, EventArgs e) {
            var wsex = _hwndSource.Handle.GetWindowLongEx();
            if (this.Owner.ResizeMode == ResizeMode.NoResize || this.Owner.ResizeMode == ResizeMode.CanMinimize) {
                wsex |= WSEX.TRANSPARENT;
            }
            else {
                wsex ^= WSEX.TRANSPARENT;
            }
            _hwndSource.Handle.SetWindowLongEx(wsex);
        }

        public void Update() {
            if (this._closing) {
                return;
            }
            if (_ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(_ownerHandle, out RECT rect)) {
                UpdateCore(rect);
            }
        }

        internal void UpdateCore(RECT rect) {
            SafeNativeMethods.SetWindowPos(_handle, _ownerHandle,
                                       (int)(_getLeft(rect)),
                                       (int)(_getTop(rect)),
                                       (int)(_getWidth(rect)),
                                       (int)(_getHeight(rect)),
                                       SWP.NOACTIVATE | SWP.NOZORDER);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == (int)WM.SHOWWINDOW) {
                if ((int)lParam == 3 && this.Visibility != Visibility.Visible) // 3 == SW_PARENTOPENING
                {
                    handled = true; //handle this message so window isn't shown until we want it to                   
                }
            }
            if (msg == (int)WM.MOUSEACTIVATE) {
                handled = true;
                return new IntPtr(3);
            }

            if (msg == (int)WM.LBUTTONDOWN) {
                var pt = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
                SafeNativeMethods.PostMessage(_ownerHandle, (uint)WM.NCLBUTTONDOWN, (IntPtr)_getHitTestValue(pt), IntPtr.Zero);
            }
            if (msg == (int)WM.NCHITTEST) {
                var ptScreen = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
                Point ptClient = PointFromScreen(ptScreen);
                Cursor cursor = _getCursor(ptClient);
                if (cursor != Cursor) Cursor = cursor;
            }

            return IntPtr.Zero;
        }

        public void Dispose() {
            _resizeModeChangeNotifier?.Dispose();
        }
    }
}
