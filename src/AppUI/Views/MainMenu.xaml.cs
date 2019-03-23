using System.Windows.Controls;

namespace NTMiner.Views {
    public partial class MainMenu : UserControl {
        public static string ViewId = nameof(MainMenu);

        public MainMenu() {
            InitializeComponent();
            if (Design.IsInDesignMode) {
                return;
            }
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
