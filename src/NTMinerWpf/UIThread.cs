using System;
using System.Windows.Threading;

namespace NTMiner {
    public static class UIThread {
        private static Action<Action> s_executor = action => action();

        public static void InitializeWithDispatcher() {
            var dispatcher = Dispatcher.CurrentDispatcher;
            s_executor = action => {
                if (dispatcher.CheckAccess())
                    action();
                else dispatcher.BeginInvoke(action);
            };
        }

        public static void Execute(this Action action) {
            s_executor(action);
        }
    }
}