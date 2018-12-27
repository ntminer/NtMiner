using NTMiner.Views;
using System.Collections.ObjectModel;

namespace NTMiner.Vms {
    public class OpenedWindowsViewModel : ViewModelBase {
        public ObservableCollection<ContainerWindowViewModel> Windows {
            get {
                return ContainerWindow.Windows;
            }
        }
    }
}
