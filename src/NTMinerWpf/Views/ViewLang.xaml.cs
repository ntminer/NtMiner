using MahApps.Metro.Controls;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views {
    public partial class ViewLang : MetroWindow {
        public static void ShowWindow(ViewLangViewModel vm) {
            ViewLang window = new ViewLang(vm);
            window.Show();
            window.Activate();
        }

        public ViewLangViewModel Vm {
            get {
                return (ViewLangViewModel)this.DataContext;
            }
        }

        private ViewLang(ViewLangViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void CbLanguage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            LangViewModel selectedItem = (LangViewModel)e.AddedItems[0];
            if (selectedItem != VirtualRoot.Lang) {
                VirtualRoot.Lang = selectedItem;
            }
            Vm.OnPropertyChanged(nameof(Vm.LangViewItemVms));
        }

        private void LangViewItemDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem != null) {
                LangViewItemViewModel poolVm = (LangViewItemViewModel)dg.SelectedItem;
                poolVm.Edit.Execute(null);
            }
        }
    }
}
