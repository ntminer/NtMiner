using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class ServerMessageEdit : UserControl {
        public static void ShowWindow(FormType formType, ServerMessageViewModel data) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "服务器消息",
                IsMaskTheParent = true,
                Width = 540,
                FormType = formType,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                ServerMessageViewModel vm = new ServerMessageViewModel(data) {
                    CloseWindow = window.Close
                };
                return new ServerMessageEdit(vm);
            }, fixedSize: true);
        }

        public ServerMessageEdit(ServerMessageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
