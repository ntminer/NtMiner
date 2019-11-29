using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GroupEdit : UserControl {
        public static void ShowWindow(FormType formType, GroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "组",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 380,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_Group"
            }, ucFactory: (window) =>
            {
                GroupViewModel vm = new GroupViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(GroupEdit));
                return new GroupEdit(vm);
            }, fixedSize: true);
        }

        public GroupEdit(GroupViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
