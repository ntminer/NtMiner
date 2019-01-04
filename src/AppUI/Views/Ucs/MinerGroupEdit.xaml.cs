using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerGroupEdit : UserControl {
        public static void ShowEditWindow(MinerGroupViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MinerGroup",
                OnOk = (uc) => {
                    MinerGroupViewModel vm = ((MinerGroupEdit)uc).Vm;
                    if (NTMinerRoot.Current.MinerGroupSet.Contains(source.Id)) {
                        Global.Execute(new UpdateMinerGroupCommand(vm));
                    }
                    else {
                        Global.Execute(new AddMinerGroupCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                MinerGroupViewModel vm = new MinerGroupViewModel(source.Id).Update(source);
                return new MinerGroupEdit(vm);
            }, fixedSize: true);
        }

        private MinerGroupViewModel Vm {
            get {
                return (MinerGroupViewModel)this.DataContext;
            }
        }
        public MinerGroupEdit(MinerGroupViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
