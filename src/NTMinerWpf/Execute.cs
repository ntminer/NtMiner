using System;
using System.Windows.Threading;

namespace NTMiner {
    public static class Execute {
        private static Action<Action> executor = action => action();

        public static void InitializeWithDispatcher() {
            var dispatcher = Dispatcher.CurrentDispatcher;
            executor = action => {
                if (dispatcher.CheckAccess())
                    action();
                else dispatcher.BeginInvoke(action);
            };
        }

        public static void OnUIThread(this Action action) {
            executor(action);
        }
    }
}