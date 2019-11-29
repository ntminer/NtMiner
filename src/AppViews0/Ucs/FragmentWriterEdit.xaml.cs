using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FragmentWriterEdit : UserControl {
        public static void ShowWindow(FormType formType, FragmentWriterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "命令行片段书写器",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 950,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_FragmentWriter"
            }, ucFactory: (window) =>
            {
                FragmentWriterViewModel vm = new FragmentWriterViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(FileWriterEdit));
                return new FragmentWriterEdit(vm);
            }, fixedSize: true);
        }

        public FragmentWriterEdit(FragmentWriterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
