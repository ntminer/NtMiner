using NTMiner.Core;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class CalcConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "收益计算器设置",
                IconName = "Icon_Calc",
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new CalcConfig();
                CalcConfigViewModels vm = (CalcConfigViewModels)uc.DataContext;
                vm.CloseWindow = () => window.Close();
                uc.ItemsControl.MouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e)=> {
                    if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
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

        private readonly List<Bus.IDelegateHandler> _handlers = new List<Bus.IDelegateHandler>();
        private CalcConfig() {
            InitializeComponent();
            VirtualRoot.On<CalcConfigSetInitedEvent>("收益计算器数据集刷新后刷新VM", LogEnum.DevConsole,
                action: message => {
                    var list = new List<CalcConfigViewModel>();
                    foreach (var item in NTMinerRoot.Current.CalcConfigSet) {
                        list.Add(new CalcConfigViewModel(item));
                    }
                    UIThread.Execute(() => {
                        Vm.CalcConfigVms = list;
                    });
                }).AddToCollection(_handlers);
            this.Unloaded += CalcConfig_Unloaded;
        }

        private void CalcConfig_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var handler in _handlers) {
                VirtualRoot.UnPath(handler);
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
