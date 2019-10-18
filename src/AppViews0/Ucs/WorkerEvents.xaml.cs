using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class WorkerEvents : UserControl {
        private WorkerEventsViewModel Vm {
            get {
                return (WorkerEventsViewModel)this.DataContext;
            }
        }

        public WorkerEvents() {
            InitializeComponent();
            this.RunOneceOnLoaded(window => {
                window.EventPath<WorkerEvent>("发生了挖矿事件后刷新Vm内存", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.WorkerEventVms.Insert(0, new WorkerEventViewModel(message.Source));
                            Vm.RefreshCount(message.Source);
                        });
                    });
            });
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
