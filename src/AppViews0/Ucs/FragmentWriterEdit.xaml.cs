using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FragmentWriterEdit : UserControl {
        public static void ShowWindow(FormType formType, FragmentWriterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "命令行片段书写器",
                FormType = formType,
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_FragmentWriter"
            }, ucFactory: (window) =>
            {
                FragmentWriterViewModel vm = new FragmentWriterViewModel(source) {
                    CloseWindow = () => window.Close()
                };
                return new FragmentWriterEdit(vm);
            }, fixedSize: true);
        }

        private FragmentWriterViewModel Vm {
            get {
                return (FragmentWriterViewModel)this.DataContext;
            }
        }

        public FragmentWriterEdit(FragmentWriterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
