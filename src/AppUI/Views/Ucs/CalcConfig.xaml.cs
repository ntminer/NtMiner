using NTMiner.Vms;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class CalcConfig : UserControl {
        public static string ViewId = nameof(CalcConfig);

        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
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

        private CalcConfig() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
