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
            DependencyProperty.Register("StartButtonBackground", typeof(SolidColorBrush), typeof(StartStopMineButton), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        [Description("开始按钮前景色"), Category("KbSkin")]
        public SolidColorBrush StartButtonForeground {
            get { return (SolidColorBrush)GetValue(StartButtonForegroundProperty); }
            set { SetValue(StartButtonForegroundProperty, value); }
        }
        public static readonly DependencyProperty StartButtonForegroundProperty =
            DependencyProperty.Register("StartButtonForeground", typeof(SolidColorBrush), typeof(StartStopMineButton), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        private StartStopMineButtonViewModel Vm {
            get {
                return MainWindowViewModel.Current.StartStopMineButtonVm;
            }
        }

        public StartStopMineButton() {
            this.DataContext = MainWindowViewModel.Current.StartStopMineButtonVm;
            InitializeComponent();
        }
    }
}
