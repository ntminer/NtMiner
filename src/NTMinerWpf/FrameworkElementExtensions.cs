using NTMiner.Bus;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NTMiner {
    public static class FrameworkElementExtensions {
        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public static DelegateHandler<TCmd> Window<TCmd>(this Window window, string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            if (window.Resources == null) {
                window.Resources = new ResourceDictionary();
            }
            List<IDelegateHandler> contextHandlers = (List<IDelegateHandler>)window.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IDelegateHandler>();
                window.Resources.Add("ntminer_contextHandlers", contextHandlers);
                window.Closed += UiElement_Closed;
            }
            return VirtualRoot.Path(description, logType, action).AddToCollection(contextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public static DelegateHandler<TEvent> On<TEvent>(this Window window, string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            if (window.Resources == null) {
                window.Resources = new ResourceDictionary();
            }
            List<IDelegateHandler> contextHandlers = (List<IDelegateHandler>)window.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IDelegateHandler>();
                window.Resources.Add("ntminer_contextHandlers", contextHandlers);
                window.Closed += UiElement_Closed; ;
            }
            return VirtualRoot.Path(description, logType, action).AddToCollection(contextHandlers);
        }

        private static void UiElement_Closed(object sender, EventArgs e) {
            Window uiElement = (Window)sender;
            List<IDelegateHandler> contextHandlers = (List<IDelegateHandler>)uiElement.Resources["ntminer_contextHandlers"];
            foreach (var handler in contextHandlers) {
                VirtualRoot.UnPath(handler);
            }
        }
    }
}
