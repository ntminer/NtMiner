using NTMiner.Bus;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NTMiner {
    public static class WindowExtension {
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
            List<IMessageHandler> contextHandlers = (List<IMessageHandler>)window.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IMessageHandler>();
                window.Resources.Add("ntminer_contextHandlers", contextHandlers);
                window.Closed += UiElement_Closed;
            }
            VirtualRoot.Path(description, logType, action).AddToCollection(contextHandlers);
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
            List<IMessageHandler> contextHandlers = (List<IMessageHandler>)window.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IMessageHandler>();
                window.Resources.Add("ntminer_contextHandlers", contextHandlers);
                window.Closed += UiElement_Closed; ;
            }
            VirtualRoot.Path(description, logType, action).AddToCollection(contextHandlers);
        }

        private static void UiElement_Closed(object sender, EventArgs e) {
            Window uiElement = (Window)sender;
            List<IMessageHandler> contextHandlers = (List<IMessageHandler>)uiElement.Resources["ntminer_contextHandlers"];
            foreach (var handler in contextHandlers) {
                VirtualRoot.UnPath(handler);
            }
        }
    }
}
