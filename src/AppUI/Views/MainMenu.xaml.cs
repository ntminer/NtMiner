using System.Windows.Controls;

namespace NTMiner.Views {
    public partial class MainMenu : UserControl {
        public MainMenu() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(nameof(MainMenu), this.Resources);
        }
    }
}
