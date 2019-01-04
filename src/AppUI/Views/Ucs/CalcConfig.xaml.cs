using NTMiner.ServiceContracts.DataObjects;
using NTMiner.Vms;
using System.Linq;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CalcConfig : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Calc",
                Width = 500,
                Height = 450,
                CloseVisible = System.Windows.Visibility.Visible,
                SaveVisible = System.Windows.Visibility.Visible,
                OnOk = (uc) => {
                    CalcConfigViewModels vm = (CalcConfigViewModels)uc.DataContext;
                    NTMinerRoot.Current.CalcConfigSet.SaveCalcConfigs(vm.CalcConfigVms.Select(a => new CalcConfigData(a)).ToList());
                    TopWindow.GetTopWindow()?.Close();
                    return true;
                }
            }, ucFactory: (window) => {
                var uc = new CalcConfig();
                uc.ItemsControl.MouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e)=> {
                    if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                        window.DragMove();
                    }
                };
                return uc;
            }, fixedSize: false);
        }

        public CalcConfigViewModels Vm {
            get {
                return (CalcConfigViewModels)this.DataContext;
            }
        }

        private CalcConfig() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(CalcConfig), this.Resources);
        }
    }
}
