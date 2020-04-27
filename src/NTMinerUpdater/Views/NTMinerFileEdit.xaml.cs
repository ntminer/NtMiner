using NTMiner.Vms;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class NTMinerFileEdit : BlankWindow {
        public NTMinerFileViewModel Vm { get; private set; }

        public NTMinerFileEdit(string iconName, NTMinerFileViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
            this.PathIcon.Data = AppUtil.GetResource<Geometry>(iconName);
            this.Owner = WpfUtil.GetTopWindow();
        }
    }
}
