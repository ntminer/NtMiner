using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class OpenedWindows : UserControl {
        public OpenedWindowsViewModel Vm {
            get {
                return (OpenedWindowsViewModel)this.DataContext;
            }
        }

        public OpenedWindows() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
