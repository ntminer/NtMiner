using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputKeywordEdit : UserControl {
        public static void ShowWindow(FormType formType, KernelOutputKeywordViewModel data) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = $"内核输出关键字({(DevMode.IsDevMode ? "服务器" : "自定义")})",
                IsMaskTheParent = true,
                Width = 540,
                FormType = formType,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                KernelOutputKeywordViewModel vm = new KernelOutputKeywordViewModel(data);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(KernelOutputKeywordEdit));
                return new KernelOutputKeywordEdit(vm);
            }, fixedSize: true);
        }

        public KernelOutputKeywordEdit(KernelOutputKeywordViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
