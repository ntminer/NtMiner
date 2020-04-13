using NTMiner.MinerStudio;
using System.Windows.Controls;

namespace NTMiner.Views.MinerStudio.Ucs {
    public partial class MinerClientOperationResults : UserControl {
        private MinerStudioRoot.MinerClientOperationResultsViewModel Vm {
            get {
                return MinerStudioRoot.MinerClientOperationResultsVm;
            }
        }

        public MinerClientOperationResults() {
            this.DataContext = MinerStudioRoot.MinerClientOperationResultsVm;
            InitializeComponent();
        }
    }
}
