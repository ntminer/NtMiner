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
                    result = window.ShowDialog();
                    maskWindow.HideMask();
                }
                else {
                    double ownerOpacity = window.Owner.Opacity;
                    window.Owner.Opacity = 0.6;
                    result = window.ShowDialog();
                    window.Owner.Opacity = ownerOpacity;
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
        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public static void CmdPath<TCmd>(this Window window, string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            if (Design.IsInDesignMode) {
                return;
            }
            if (window.Resources == null) {
                window.Resources = new ResourceDictionary();
            }
            List<IHandlerId> contextHandlers = (List<IHandlerId>)window.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IHandlerId>();
                window.Resources.Add("ntminer_contextHandlers", contextHandlers);
                window.Closed += UiElement_Closed;
            }
            VirtualRoot.CreatePath(description, logType, action).AddToCollection(contextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public static void EventPath<TEvent>(this Window window, string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            if (Design.IsInDesignMode) {
                return;
            }
            if (window.Resources == null) {
                window.Resources = new ResourceDictionary();
            }
            List<IHandlerId> contextHandlers = (List<IHandlerId>)window.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IHandlerId>();
                window.Resources.Add("ntminer_contextHandlers", contextHandlers);
                window.Closed += UiElement_Closed; ;
            }
            VirtualRoot.CreatePath(description, logType, action).AddToCollection(contextHandlers);
        }

        private static void UiElement_Closed(object sender, EventArgs e) {
            Window uiElement = (Window)sender;
            List<IHandlerId> contextHandlers = (List<IHandlerId>)uiElement.Resources["ntminer_contextHandlers"];
            foreach (var handler in contextHandlers) {
                VirtualRoot.DeletePath(handler);
            }
        }
    }
}
