using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SysDicItemEdit : UserControl {
        public static void ShowEditWindow(SysDicItemViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_SysDic",
                OnOk = (uc) => {
                    var vm = ((SysDicItemEdit)uc).Vm;
                    if (NTMinerRoot.Current.SysDicItemSet.ContainsKey(source.Id)) {
                        Global.Execute(new UpdateSysDicItemCommand(vm));
                    }
                    else {
                        Global.Execute(new AddSysDicItemCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                SysDicItemViewModel vm = new SysDicItemViewModel(source.Id).Update(source);
                return new SysDicItemEdit(vm);
            }, fixedSize: true);
        }

        private SysDicItemViewModel Vm {
            get {
                return (SysDicItemViewModel)this.DataContext;
            }
        }

        public SysDicItemEdit(SysDicItemViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
