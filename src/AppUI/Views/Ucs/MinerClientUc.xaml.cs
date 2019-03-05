using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientUc : UserControl {
        public static readonly string ViewId = nameof(MinerClientUc);

        public static void ShowWindow(MinerClientViewModel vm) {
            ResourceDictionary resourceDictionary;
            if (ResourceDictionarySet.Instance.TryGetResourceDic(ViewId, out resourceDictionary)) {
                resourceDictionary["WindowTitle"] = vm.MinerIp;
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                IconName = "Icon_Miner",
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new MinerClientUc(vm), fixedSize: true);
        }

        public MinerClientViewModel Vm {
            get {
                return (MinerClientViewModel)this.DataContext;
            }
        }

        public MinerClientUc(MinerClientViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.Resources["WindowTitle"] = vm.MinerIp;
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void TbIp_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            MinerClientViewModel vm = (MinerClientViewModel)((FrameworkElement)sender).Tag;
            vm.RemoteDesktop.Execute(null);
            e.Handled = true;
        }
    }
}
