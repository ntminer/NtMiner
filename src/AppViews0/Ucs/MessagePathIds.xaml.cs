using NTMiner.Hub;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MessagePathIds : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "集线器",
                IconName = "Icon_Logo",
                Width = 1200,
                Height = 600,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new MessagePathIds();
                return uc;
            }, fixedSize: false);
        }

        public MessagePathIdsViewModel Vm {
            get {
                return (MessagePathIdsViewModel)this.DataContext;
            }
        }

        public MessagePathIds() {
            InitializeComponent();
            this.OnLoaded(onLoad: window => {
                VirtualRoot.MessageHub.PathAdded += OnPathConnected;
                VirtualRoot.MessageHub.PathRemoved += OnPathDisconnected;
            }, onUnload: window => {
                VirtualRoot.MessageHub.PathAdded -= OnPathConnected;
                VirtualRoot.MessageHub.PathRemoved -= OnPathDisconnected;
            });
        }

        private void OnPathConnected(IMessagePathId pathId) {
            UIThread.Execute(() => () => {
                Vm.PathIds.Add(pathId);
            });
        }

        private void OnPathDisconnected(IMessagePathId pathId) {
            UIThread.Execute(() => () => {
                Vm.PathIds.Remove(pathId);
            });
        }
    }
}
