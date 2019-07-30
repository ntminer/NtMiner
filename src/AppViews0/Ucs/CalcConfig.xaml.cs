using NTMiner.Core;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class CalcConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "收益计算器设置",
                IconName = "Icon_Calc",
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new CalcConfig();
                CalcConfigViewModels vm = (CalcConfigViewModels)uc.DataContext;
                vm.CloseWindow = () => window.Close();
                uc.ItemsControl.MouseDown += (object sender, MouseButtonEventArgs e)=> {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                return uc;
            }, fixedSize: true);
        }

        public CalcConfigViewModels Vm {
            get {
                return (CalcConfigViewModels)this.DataContext;
            }
        }

        private CalcConfig() {
            InitializeComponent();
            this.RunOneceOnLoaded((window) => {
                window.On<CalcConfigSetInitedEvent>("收益计算器数据集刷新后刷新VM", LogEnum.DevConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            Vm.Refresh();
                        });
                    });
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
