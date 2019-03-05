using System;
using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    public class ConsoleHandler {
        private static class NativeMethods {
            [DllImport("kernel32.dll")]
            public static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        }

        public delegate bool ControlCtrlDelegate(int ctrlType);

        private bool HandlerRoutine(int ctrlType) {
            switch (ctrlType) {
                case 0:
                    _onClose?.Invoke(); //Ctrl+C关闭  
                    break;
                case 2:
                    _onClose?.Invoke();//按控制台关闭按钮关闭  
                    break;
            }
            return false;
        }

        private readonly Action _onClose;
        private ConsoleHandler(Action onClose) {
            _onClose = onClose;
            ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);
            NativeMethods.SetConsoleCtrlHandler(cancelHandler, true);
        }

        public static void Register(Action onClose) {
            new ConsoleHandler(onClose);
        }
    }
}
