using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class StateBar : UserControl {
        public static string ViewId = nameof(StateBar);

        private MinerProfileViewModel Vm {
            get {
                return (MinerProfileViewModel)this.DataContext;
            }
        }

        public StateBar() {
            this.DataContext = MinerProfileViewModel.Current;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
