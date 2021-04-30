using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MqCountsPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "Mq Counts",
                IconName = "Icon_RabbitMq",
                Width = 1400,
                Height = 800,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => new MqCountsPage());
        }

        public MqCountsPageViewModel Vm { get; private set; }

        public MqCountsPage() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new MqCountsPageViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded(window => {
                this.Load();
                window.BuildEventPath<Per10SecondEvent>("周期自动刷新", LogEnum.None, this.GetType(), PathPriority.Normal, message => {
                    this.Load();
                });
            });
        }

        private void Load() {
            RpcRoot.OfficialServer.AdminService.GetMqCountsAsync((response, e) => {
                if (response.IsSuccess() && response.Data != null && response.Data.Length != 0) {
                    this.Vm.SetData(response.Data);
                }
            });
        }
    }
}
