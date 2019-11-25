using NTMiner.Views;
using NTMiner.Vms;

namespace NTMiner {
    public static class ViewModelExtension {
        // 起编译时免导入DialogWindow所在的NTMiner.Views命名空间的作用
        public static void ShowDialog(this ViewModelBase vm, DialogWindowViewModel config) {
            DialogWindow.ShowSoftDialog(config);
        }
    }
}
