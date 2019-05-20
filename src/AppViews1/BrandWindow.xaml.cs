using NTMiner.Vms;

namespace NTMiner.Views {
    public partial class BrandWindow : BlankWindow {
        public BrandWindowViewModel Vm {
            get {
                return (BrandWindowViewModel)this.DataContext;
            }
        }

        public BrandWindow() {
            InitializeComponent();
        }
    }
}
