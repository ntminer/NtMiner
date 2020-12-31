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
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed
            }, ucFactory: (window) =>
            {
                ServerMessageViewModel vm = new ServerMessageViewModel(data);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new ServerMessageEdit(vm);
            }, fixedSize: true);
        }

        public ServerMessageEdit(ServerMessageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void FindKernel_Click(object sender, System.Windows.RoutedEventArgs e) {
            string keyword = this.TbContent.SelectedText;
            KernelsWindow.ShowWindow(keyword);
        }
    }
}
