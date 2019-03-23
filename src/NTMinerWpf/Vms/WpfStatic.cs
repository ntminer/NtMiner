using NTMiner.Views;
using System.Windows.Input;

namespace NTMiner.Vms {
    public static class WpfStatic {
        public static ICommand ShowLangViewItems { get; private set; } = new DelegateCommand<string>((viewId) => {
            ViewLang.ShowWindow(new ViewLangViewModel(viewId));
        });
    }
}
