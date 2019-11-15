using NTMiner.Bus;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NTMiner {
    public static class WindowExtension {
        /// <summary>
        /// 基于鼠标位置放置窗口
        /// </summary>
        /// <param name="window"></param>
        public static void MousePosition(this Window window) {
            if (window.Owner == null) {
                return;
            }
            if (SafeNativeMethods.GetCursorPos(out POINT pt)) {
                var width = window.Width.Equals(double.NaN) ? 400 : window.Width;
                var height = window.Height.Equals(double.NaN) ? 200 : window.Height;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                double left = pt.X - width / 2;
                double top = pt.Y + 20;
                if (left < window.Owner.Left) {
                    left = window.Owner.Left;
                }
                var ownerTop = window.Owner.Top;
                var ownerLeft = window.Owner.Left;
                if (window.Owner.WindowState == WindowState.Maximized) {
                    ownerTop = 0;
                    ownerLeft = 0;
                }
                var over = top + height - ownerTop - window.Owner.Height;
                if (over > 0) {
                    top = pt.Y - height - 20;
                }
                over = left + width - ownerLeft - window.Owner.Width;
                if (over > 0) {
                    left -= over;
                }
                window.Left = left;
                window.Top = top;
            }
        }

        public static bool? ShowDialogEx(this Window window) {
            bool? result;
            if (window.Owner == null) {
                var owner = WpfUtil.GetTopWindow();
                if (owner != window) {
                    window.Owner = owner;
                }
            }
            if (window.Owner != null) {
                if (window.Owner is IMaskWindow maskWindow) {
                    maskWindow.ShowMask();
                    window.Closing += (sender, e) => {
                        maskWindow.HideMask();
                    };
                    result = window.ShowDialog();
                }
                else {
                    double ownerOpacity = window.Owner.Opacity;
                    window.Owner.Opacity = 0.6;
                    window.Closing += (sender, e) => {
                        window.Owner.Opacity = ownerOpacity;
                    };
                    result = window.ShowDialog();
                }
            }
            else {
                result = window.ShowDialog();
            }
            return result;
        }

        public static void ShowWindow(this Window window, bool isToggle) {
            if (isToggle) {
                if (window.IsVisible && window.WindowState != WindowState.Minimized) {
                    window.Hide();
                }
                else {
                    window.Show();
                    if (window.WindowState == WindowState.Minimized) {
                        window.WindowState = WindowState.Normal;
                    }
                }
            }
            else {
                window.Show();
                if (window.WindowState == WindowState.Minimized) {
                    window.WindowState = WindowState.Normal;
                }
            }
        }

        private const string messagePathIdsResourceKey = "messagePathIds";

        // 因为是上下文路径，无需返回路径标识
        public static void BuildCmdPath<TCmd>(this Window window, string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            if (window.Resources == null) {
                window.Resources = new ResourceDictionary();
            }
            List<IMessagePathId> messagePathIds = (List<IMessagePathId>)window.Resources[messagePathIdsResourceKey];
            if (messagePathIds == null) {
                messagePathIds = new List<IMessagePathId>();
                window.Resources.Add(messagePathIdsResourceKey, messagePathIds);
                window.Closed += UiElement_Closed;
            }
            var messagePathId = VirtualRoot.BuildPath(description, logType, action);
            messagePathIds.Add(messagePathId);
        }

        // 因为是上下文路径，无需返回路径标识
        public static void BuildEventPath<TEvent>(this Window window, string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            if (window.Resources == null) {
                window.Resources = new ResourceDictionary();
            }
            List<IMessagePathId> messagePathIds = (List<IMessagePathId>)window.Resources[messagePathIdsResourceKey];
            if (messagePathIds == null) {
                messagePathIds = new List<IMessagePathId>();
                window.Resources.Add(messagePathIdsResourceKey, messagePathIds);
                window.Closed += UiElement_Closed; ;
            }
            var messagePathId = VirtualRoot.BuildPath(description, logType, action);
            messagePathIds.Add(messagePathId);
        }

        private static void UiElement_Closed(object sender, EventArgs e) {
            Window uiElement = (Window)sender;
            List<IMessagePathId> messageIds = (List<IMessagePathId>)uiElement.Resources[messagePathIdsResourceKey];
            foreach (var handler in messageIds) {
                VirtualRoot.DeletePath(handler);
            }
        }
    }
}
