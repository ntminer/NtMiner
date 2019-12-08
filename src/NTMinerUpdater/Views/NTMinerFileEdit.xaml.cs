using NTMiner.Vms;
using System.Windows;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class NTMinerFileEdit : BlankWindow {
        public NTMinerFileViewModel Vm {
            get {
                return (NTMinerFileViewModel)this.DataContext;
            }
        }

        public NTMinerFileEdit(string iconName, NTMinerFileViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            this.PathIcon.Data = (Geometry)Application.Current.Resources[iconName];
            this.Owner = WpfUtil.GetTopWindow();
        }
    }
}
