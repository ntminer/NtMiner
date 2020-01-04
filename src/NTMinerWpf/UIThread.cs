using System;
using System.Windows.Threading;

namespace NTMiner {
    public static class UIThread {
        private static Dispatcher _dispatcher;
        public static void InitializeWithDispatcher() {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Write.UIThreadId = _dispatcher.Thread.ManagedThreadId;
        }

        /// <summary>
        /// 因为该方法可能会在非UI线程被调用，所以是这个风格。
        /// 详解：当以UIThread.Execute(Vm.Method1)这个风格调用时，因为Vm实例可能来自
        /// 于(TVm)this.DataContext，而this是DependencyObject是不能在非UI线程访问的。
        /// </summary>
        public static void Execute(Func<Action> getAction) {
            if (_dispatcher.CheckAccess()) {
                getAction()();
            }
            else {
                _dispatcher.BeginInvoke(new Action(()=> {
                    getAction()();
                }));
            }
        }
    }
}