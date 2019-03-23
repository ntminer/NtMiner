using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class AboutPage : UserControl {
        public static string ViewId = nameof(AboutPage);

        public static void ShowWindow(string appType) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_About",
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
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
