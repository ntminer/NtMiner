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
            DependencyProperty.Register(nameof(StartButtonBackground), typeof(SolidColorBrush), typeof(StartStopMineButton), new PropertyMetadata(WpfUtil.TransparentBrush));
        
        [Description("开始按钮前景色"), Category("KbSkin")]
        public SolidColorBrush StartButtonForeground {
            get { return (SolidColorBrush)GetValue(StartButtonForegroundProperty); }
            set { SetValue(StartButtonForegroundProperty, value); }
        }
        public static readonly DependencyProperty StartButtonForegroundProperty =
            DependencyProperty.Register(nameof(StartButtonForeground), typeof(SolidColorBrush), typeof(StartStopMineButton), new PropertyMetadata(WpfUtil.TransparentBrush));

        [Description("是否显示按钮文字"), Category("KbSkin")]
        public Visibility TextVisible {
            get { return (Visibility)GetValue(TextVisibleProperty); }
            set { SetValue(TextVisibleProperty, value); }
        }
        public static readonly DependencyProperty TextVisibleProperty =
            DependencyProperty.Register(nameof(TextVisible), typeof(Visibility), typeof(StartStopMineButton), new PropertyMetadata(Visibility.Visible));

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
