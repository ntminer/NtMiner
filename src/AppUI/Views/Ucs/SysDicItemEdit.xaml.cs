using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SysDicItemEdit : UserControl {
        public static void ShowEditWindow(SysDicItemViewModel source) {
            string title;
            if (!DevMode.IsDevMode) {
                title = "字典项详情";
            }
            else {
                if (NTMinerRoot.Current.SysDicItemSet.ContainsKey(source.Id)) {
                    title = "编辑字典项";
                }
                else {
                    title = "添加字典项";
                }
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
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
