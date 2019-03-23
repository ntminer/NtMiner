using MahApps.Metro.Controls;
using NTMiner.Vms;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class LangViewItemEdit : MetroWindow {
        public static string ViewId = nameof(LangViewItemEdit);

        public static void ShowWindow(LangViewItemViewModel vm) {
            LangViewItemEdit window = new LangViewItemEdit(vm);
            window.ShowDialog();
        }

        private LangViewItemEdit(LangViewItemViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
