using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FileWriterEdit : UserControl {
        public static void ShowWindow(FormType formType, FileWriterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "文件书写器",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 950,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_FileWriter"
            }, ucFactory: (window) =>
            {
                FileWriterViewModel vm = new FileWriterViewModel(source);
                window.AddCloseWindowOnecePath(vm.Id);
                return new FileWriterEdit(vm);
            }, fixedSize: true);
        }

        private FileWriterViewModel Vm {
            get {
                return (FileWriterViewModel)this.DataContext;
            }
        }

        public FileWriterEdit(FileWriterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
