using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    public class ConsoleHandler {
        private static class NativeMethods {
            [DllImport("kernel32.dll")]
            public static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        }

        public delegate bool ControlCtrlDelegate(int ctrlType);

        // 静态的，调用前不能被垃圾回收
        private static readonly List<Action> _actions = new List<Action>();
        private static readonly ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        private static bool HandlerRoutine(int ctrlType) {
            switch (ctrlType) {
                case 0:// Ctrl+C关闭  
                    foreach (var item in _actions) {
                        item?.Invoke();
                    }
                    break;
                case 2:// 按控制台关闭按钮关闭  
                    foreach (var item in _actions) {
                        item?.Invoke();
                    }
                    break;
            }
            return false;
        }

        public static void Register(Action onClose) {
            _actions.Add(onClose);
            NativeMethods.SetConsoleCtrlHandler(cancelHandler, true);
        }
    }
}
