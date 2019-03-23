using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelHelpPage : UserControl {
        public static string ViewId = nameof(KernelHelpPage);

        public static void ShowWindow(string title, string helpText) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Help",
                Width = 700,
                Height = 500,
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) =>
            {
                return new KernelHelpPage(new KernelHelpPageViewModel() {
                    HelpText = helpText
                });
            }, fixedSize: false);
        }

        public KernelHelpPageViewModel Vm {
            get {
                return (KernelHelpPageViewModel)this.DataContext;
            }
        }

        public KernelHelpPage(KernelHelpPageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
