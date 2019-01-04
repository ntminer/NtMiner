using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MineWorkEdit : UserControl {
        public static void ShowEditWindow(MineWorkViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_MineWork",
                OnOk = (uc) => {
                    MineWorkViewModel vm = ((MineWorkEdit)uc).Vm;
                    if (NTMinerRoot.Current.MineWorkSet.Contains(source.Id)) {
                        Global.Execute(new UpdateMineWorkCommand(vm));
                    }
                    else {
                        Global.Execute(new AddMineWorkCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                MineWorkViewModel vm = new MineWorkViewModel(source.Id).Update(source);
                return new MineWorkEdit(vm);
            }, fixedSize: true);
        }

        private MineWorkViewModel Vm {
            get {
                return (MineWorkViewModel)this.DataContext;
            }
        }
        public MineWorkEdit(MineWorkViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(MineWorkEdit), this.Resources);
        }
    }
}
