using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class Enviroment : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "电脑概览",
                IconName = "Icon_Enviroment",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => new Enviroment(), fixedSize: true);
        }

        private EnviromentViewModel Vm {
            get {
                return (EnviromentViewModel)this.DataContext;
            }
        }

        public Enviroment() {
            InitializeComponent();
        }
    }
}
