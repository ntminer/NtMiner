using System;
using System.Windows.Threading;

namespace NTMiner {
    public static class UIThread {
        private static Dispatcher _dispatcher;

        public static Dispatcher Dispatcher {
            get { return _dispatcher; }
        }

        /// <summary>
        /// 执行两个操作：
        /// 1，记下对Dispatcher.CurrentDispatcher的引用，因为Splash会另开一个UI线程，防止访问到Splash线程的Dispatcher.CurrentDispatcher；
        /// 2，设置Writer.UIThreadId；
        /// </summary>
        public static void InitializeWithDispatcher(Dispatcher dispatcher) {
            _dispatcher = dispatcher;
            Write.SetUIThreadId(_dispatcher.Thread.ManagedThreadId);
        }

        public static bool CheckAccess() {
            if (_dispatcher == null) {
                throw new InvalidProgramException();
            }
            return _dispatcher.CheckAccess();
        }

        /// <summary>
        /// 因为该方法可能会在非UI线程被调用，所以是这个风格。
        /// 详解：当以UIThread.Execute(Vm.Method1)这个风格调用时，因为Vm实例可能来自
        /// 于(TVm)this.DataContext，而this.DataContext是依赖属性，依赖属性在Wpf中是
        /// 通过GetValue()静态方法访问的，而GetValue()方法中会VerifyAccess()。
        /// </summary>
        public static void Execute(Action action) {
            if (CheckAccess()) {
                action();
            }
            else {
                _dispatcher.BeginInvoke(new Action(()=> {
                    action();
                }));
            }
        }
    }
}