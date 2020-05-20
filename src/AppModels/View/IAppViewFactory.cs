using System.Windows;

namespace NTMiner.View {
    // 这里搞了一层抽象是因为可能会提供不同界面的程序而不仅仅是皮肤不同，从AppView0的命名也能看出可能会有AppView1，但可预见的时间里不会有AppView1。
    public interface IAppViewFactory {
        /// <summary>
        /// 一个程序只有一个主窗口，所以内部会缓存输出的mainWidnow对象，只有一个mainWindow对象。
        /// </summary>
        /// <param name="isToggle"></param>
        /// <param name="mainWindow"></param>
        void ShowMainWindow(bool isToggle, out Window mainWindow);

        /// <summary>
        /// 调用该方法唤醒已经存在的另一个进程的界面。
        /// 因为挖矿端和群控客户端程序都是单例的进程，如果已经有一个进程存在了试图再打开一个进程时实际上是唤醒另一个进程的界面。
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appType"></param>
        void ShowMainWindow(Application app, NTMinerAppType appType);

        /// <summary>
        /// 创建一些与界面有关的路径。此类路径主要就是“打开某个页面”这种路径。
        /// </summary>
        void BuildPaths();
    }
}
