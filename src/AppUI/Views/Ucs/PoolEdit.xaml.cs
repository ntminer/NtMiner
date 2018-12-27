using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolEdit : UserControl {
        public static void ShowEditWindow(PoolViewModel source) {
            string title;
            string iconName;
            if (NTMinerRoot.Current.PoolSet.Contains(source.Id)) {
                title = "编辑矿池";
                iconName = "Icon_Edit";
            }
            else {
                title = "添加矿池";
                iconName = "Icon_Add";
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
                IconName = iconName,
                IsDialogWindow = true,
                SaveVisible = source.IsReadOnly ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                OnOk = (uc) => {
                    var vm = ((PoolEdit)uc).Vm;
                    if (NTMinerRoot.Current.PoolSet.Contains(source.Id)) {
                        Global.Execute(new UpdatePoolCommand(vm));
                    }
                    else {
                        Global.Execute(new AddPoolCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                PoolViewModel vm = new PoolViewModel(source);
                return new PoolEdit(vm);
            }, fixedSize: true);
        }

        private PoolViewModel Vm {
            get {
                return (PoolViewModel)this.DataContext;
            }
        }

        public PoolEdit(PoolViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
