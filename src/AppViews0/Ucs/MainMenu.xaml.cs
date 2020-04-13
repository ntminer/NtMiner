using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MainMenu : UserControl {
        public MainMenu() {
            this.DataContext = MainMenuViewModel.Instance;
            InitializeComponent();
        }
    }
}
