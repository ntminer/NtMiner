using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class LogFiles : UserControl {
        public LogFilesViewModel Vm { get; private set; }

        public LogFiles() {
            this.Vm = new LogFilesViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        private void BtnOpenKernelLogFile_Click(object sender, RoutedEventArgs e) {
            string fileFullName = Vm.GetLatestLogFileFullName();
            if (string.IsNullOrEmpty(fileFullName)) {
                VirtualRoot.Out.ShowWarn("没有日志", autoHideSeconds: 2);
                return;
            }
            Vm.OpenLogFileByNpp(fileFullName);
        }

        private void ButtonLogFiles_Click(object sender, RoutedEventArgs e) {
            PopupLogFiles.IsOpen = true;
            Vm.RefreshLogFiles();
        }

        private void LogFilesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<LogFilesViewModel.LogFile>(sender, e, t => {
                Vm.OpenLogFileByNpp(t.FileFullName);
            });
        }
    }
}
