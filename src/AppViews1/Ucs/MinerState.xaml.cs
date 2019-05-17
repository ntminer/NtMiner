using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerState : UserControl {
        private MinerStateViewModel Vm {
            get {
                return (MinerStateViewModel)this.DataContext;
            }
        }

        public MinerState() {
            InitializeComponent();
            this.SizeChanged += (object sender, System.Windows.SizeChangedEventArgs e)=> {
                const double width = 720;
                if (e.NewSize.Width < width) {
                    Vm.SideWidth = 160;
                }
                else if (e.NewSize.Width >= width) {
                    Vm.SideWidth = MinerStateViewModel.DefaultSideWidth;
                }
            };
        }
    }
}
