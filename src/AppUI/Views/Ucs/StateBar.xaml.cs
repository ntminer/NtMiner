using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class StateBar : UserControl {
        public static string ViewId = nameof(StateBar);

        private Vm Vm {
            get {
                return (Vm)this.DataContext;
            }
        }

        public StateBar() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
