using NTMiner.Bus;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MessagePathIds : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "消息路径标识集",
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
            this.RunOneceOnLoaded(onLoad: window => {
                VirtualRoot.MessageDispatcher.Added += OnPathConnected;
                VirtualRoot.MessageDispatcher.Removed += OnPathDisconnected;
            }, onUnload: window => {
                VirtualRoot.MessageDispatcher.Added -= OnPathConnected;
                VirtualRoot.MessageDispatcher.Removed -= OnPathDisconnected;
            });
        }

        private void OnPathConnected(IMessagePathId pathId) {
            UIThread.Execute(() => {
                Vm.PathIds.Add(pathId);
            });
        }

        private void OnPathDisconnected(IMessagePathId pathId) {
            UIThread.Execute(() => {
                Vm.PathIds.Remove(pathId);
            });
        }
    }
}
