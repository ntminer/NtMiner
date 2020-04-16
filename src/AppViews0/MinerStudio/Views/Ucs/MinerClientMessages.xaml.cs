using NTMiner.MinerStudio;
using System.Windows.Controls;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class MinerClientMessages : UserControl {
        private MinerStudioRoot.MinerClientMessagesViewModel Vm {
            get {
                return MinerStudioRoot.MinerClientMessagesVm;
            }
        }

        public MinerClientMessages() {
            this.DataContext = MinerStudioRoot.MinerClientMessagesVm;
            InitializeComponent();
        }
    }
}
