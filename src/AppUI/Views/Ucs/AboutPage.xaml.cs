using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class AboutPage : UserControl {
        public static void ShowWindow(string appType) {
            string iconImage = "/NTMinerWpf;component/Styles/Images/logo32.png";
            if (appType == "ControlCenter") {
                iconImage = "/NTMinerWpf;component/Styles/Images/cc32.png";
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "关于",
                IconImage = iconImage,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) => new AboutPage(appType), fixedSize: true);
        }

        public AboutPageViewModel Vm {
            get {
                return (AboutPageViewModel)this.DataContext;
            }
        }

        public AboutPage(string appType) {
            InitializeComponent();
            if (appType == "ControlCenter") {
                Vm.ImageSource = "/NTMinerWpf;component/Styles/Images/cc128.png";
            }
        }
    }
}
