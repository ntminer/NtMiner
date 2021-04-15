using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class CalcConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "收益计算器设置",
                IconName = "Icon_Calc",
                Width = 600,
                Height = 600,
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new CalcConfig();
                window.BuildCloseWindowOnecePath(uc.Vm.Id);
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e)=> {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                return uc;
            }, fixedSize: true);
        }

        public CalcConfigViewModels Vm { get; private set; }

        private CalcConfig() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new CalcConfigViewModels();
            this.DataContext = this.Vm;
            InitializeComponent();
            this.OnLoaded((window) => {
                window.BuildEventPath<CalcConfigSetInitedEvent>("收益计算器数据集刷新后刷新VM", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        Vm.Refresh();
                    });
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
