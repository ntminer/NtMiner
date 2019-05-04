using NTMiner.Bus;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NTMiner.Views {
    public static class FrameworkElementExtensions {
        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        internal static DelegateHandler<TCmd> Window<TCmd>(this FrameworkElement uiElement, string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            if (uiElement.Resources == null) {
                uiElement.Resources = new ResourceDictionary();
            }
            List<IDelegateHandler> contextHandlers = (List<IDelegateHandler>)uiElement.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IDelegateHandler>();
                uiElement.Resources.Add("ntminer_contextHandlers", contextHandlers);
                uiElement.Unloaded += UiElement_Unloaded;
            }
            return VirtualRoot.Path(description, logType, action).AddToCollection(contextHandlers);
        }

        private static void UiElement_Unloaded(object sender, RoutedEventArgs e) {
            FrameworkElement uiElement = (FrameworkElement)sender;
            List<IDelegateHandler> contextHandlers = (List<IDelegateHandler>)uiElement.Resources["ntminer_contextHandlers"];
            foreach (var handler in contextHandlers) {
                VirtualRoot.UnPath(handler);
            }
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        internal static DelegateHandler<TEvent> On<TEvent>(this FrameworkElement uiElement, string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            if (uiElement.Resources == null) {
                uiElement.Resources = new ResourceDictionary();
            }
            List<IDelegateHandler> contextHandlers = (List<IDelegateHandler>)uiElement.Resources["ntminer_contextHandlers"];
            if (contextHandlers == null) {
                contextHandlers = new List<IDelegateHandler>();
                uiElement.Resources.Add("ntminer_contextHandlers", contextHandlers);
                uiElement.Unloaded += UiElement_Unloaded;
            }
            return VirtualRoot.Path(description, logType, action).AddToCollection(contextHandlers);
        }
    }
}
