using NTMiner.Core;
using System.Windows;

namespace NTMiner.View {
    public interface IAppViewFactory {
        void ShowMainWindow(bool isToggle, out Window mainWindow);
        void ShowMainWindow(Application app, NTMinerAppType appType);
        void Link();
    }
}
