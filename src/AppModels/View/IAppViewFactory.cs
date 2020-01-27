using NTMiner.Core;
using System.Windows;

namespace NTMiner.View {
    public interface IAppViewFactory {
        void ShowMainWindow(bool isToggle);
        void ShowMainWindow(Application app, NTMinerAppType appType);
        void Link();
    }
}
