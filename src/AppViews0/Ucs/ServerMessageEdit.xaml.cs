using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ServerMessageEdit : UserControl {
        public static void ShowWindow(FormType formType, ServerMessageViewModel data) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "服务器消息",
                IsDialogWindow = true,
                Width = 500,
                FormType = formType,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                ServerMessageViewModel vm = new ServerMessageViewModel(data) {
                    CloseWindow = () => window.Close()
                };
                return new ServerMessageEdit(vm);
            }, fixedSize: true);
        }

        private ServerMessageViewModel Vm {
            get {
                return (ServerMessageViewModel)this.DataContext;
            }
        }

        public ServerMessageEdit(ServerMessageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
