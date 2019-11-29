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
                ServerMessageViewModel vm = new ServerMessageViewModel(data);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(ServerMessageEdit));
                return new ServerMessageEdit(vm);
            }, fixedSize: true);
        }

        public ServerMessageEdit(ServerMessageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
