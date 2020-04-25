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
        /// 注意传入的action可能是Vm的成员方法，所以必须确保访问Vm的时候不能访问到DependencyObject，所以整个系统中要杜绝在View.xacml中构造Vm。
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