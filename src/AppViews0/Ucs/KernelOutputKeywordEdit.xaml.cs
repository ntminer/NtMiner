using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputKeywordEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputKeywordViewModel data) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = $"内核输出关键字({(DevMode.IsDevMode ? "服务器" : "自定义")})",
                IsDialogWindow = true,
                Width = 540,
                FormType = formType,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                KernelOutputKeywordViewModel vm = new KernelOutputKeywordViewModel(data) {
                    CloseWindow = () => window.Close()
                };
                return new KernelOutputKeywordEdit(vm);
            }, fixedSize: true);
        }

        public KernelOutputKeywordEdit(KernelOutputKeywordViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
