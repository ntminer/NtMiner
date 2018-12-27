using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class SysDicEdit : UserControl {
        public static void ShowEditWindow(SysDicViewModel source) {
            string title;
            if (!DevMode.IsDevMode) {
                title = "字典详情";
            }
            else {
                if (NTMinerRoot.Current.SysDicSet.ContainsKey(source.Id)) {
                    title = "编辑字典";
                }
                else {
                    title = "添加字典";
                }
            }
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = title,
                IsDialogWindow = true,
                SaveVisible = System.Windows.Visibility.Visible,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_SysDic",
                OnOk = (uc) => {
                    var vm = ((SysDicEdit)uc).Vm;
                    if (NTMinerRoot.Current.SysDicSet.ContainsKey(source.Id)) {
                        Global.Execute(new UpdateSysDicCommand(vm));
                    }
                    else {
                        Global.Execute(new AddSysDicCommand(vm));
                    }
                    return true;
                }
            }, ucFactory: (window) =>
            {
                SysDicViewModel vm = new SysDicViewModel(source.Id).Update(source);
                return new SysDicEdit(vm);
            }, fixedSize: true);
        }

        private SysDicViewModel Vm {
            get {
                return (SysDicViewModel)this.DataContext;
            }
        }

        public SysDicEdit(SysDicViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
