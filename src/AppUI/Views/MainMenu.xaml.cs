using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views {
    public partial class MainMenu : UserControl {
        public MainMenuViewModel Vm {
            get {
                return (MainMenuViewModel)this.DataContext;
            }
        }

        public MainMenu() {
            InitializeComponent();
            MenuItemMinerStudioGroup.Visibility = IsMinerStudioDevVisible;
            MenuItemMinerClientGroup.Visibility = IsMinerClientVisible;
        }

        private Visibility IsMinerClientVisible {
            get {
                if (AppContext.Current.IsMinerClient) {
                    return Visibility.Visible;
                }
                else {
                    return Visibility.Collapsed;
                }
            }
        }

        private Visibility IsMinerStudioDevVisible {
            get {
                if (!DevMode.IsDevMode) {
                    return Visibility.Collapsed;
                }
                if (VirtualRoot.IsMinerStudio) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
