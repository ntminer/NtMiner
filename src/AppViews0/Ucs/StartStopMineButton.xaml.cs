using NTMiner.Vms;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class StartStopMineButton : UserControl {
        [Description("开始按钮背景色"), Category("KbSkin")]
        public SolidColorBrush StartButtonBackground {
            get { return (SolidColorBrush)GetValue(StartButtonBackgroundProperty); }
            set { SetValue(StartButtonBackgroundProperty, value); }
        }
        public static readonly DependencyProperty StartButtonBackgroundProperty =
            DependencyProperty.Register("StartButtonBackground", typeof(SolidColorBrush), typeof(StartStopMineButton), new PropertyMetadata(Wpf.Util.TransparentBrush));


        [Description("开始按钮前景色"), Category("KbSkin")]
        public SolidColorBrush StartButtonForeground {
            get { return (SolidColorBrush)GetValue(StartButtonForegroundProperty); }
            set { SetValue(StartButtonForegroundProperty, value); }
        }
        public static readonly DependencyProperty StartButtonForegroundProperty =
            DependencyProperty.Register("StartButtonForeground", typeof(SolidColorBrush), typeof(StartStopMineButton), new PropertyMetadata(Wpf.Util.TransparentBrush));

        private StartStopMineButtonViewModel Vm {
            get {
                return AppContext.Instance.StartStopMineButtonVm;
            }
        }

        public StartStopMineButton() {
            this.DataContext = AppContext.Instance.StartStopMineButtonVm;
            InitializeComponent();
        }
    }
}
