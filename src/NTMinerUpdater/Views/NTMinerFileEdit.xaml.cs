using MahApps.Metro.Controls;
using NTMiner.Vms;
using NTMiner.Wpf;
using System.Windows;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class NTMinerFileEdit : MetroWindow {
        public NTMinerFileViewModel Vm {
            get {
                return (NTMinerFileViewModel)this.DataContext;
            }
        }

        public NTMinerFileEdit(string title, string iconName, NTMinerFileViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.TbTitle.Text = title;
            this.PathIcon.Data = (Geometry)Application.Current.Resources[iconName];
            this.Owner = TopWindow.GetTopWindow();
        }

        private void MetroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
