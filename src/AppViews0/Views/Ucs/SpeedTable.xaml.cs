using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class SpeedTable : UserControl {
        public Visibility IsOverClockVisible {
            get { return (Visibility)GetValue(IsOverClockVisibleProperty); }
            set { SetValue(IsOverClockVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsOverClockVisibleProperty =
            DependencyProperty.Register(nameof(IsOverClockVisible), typeof(Visibility), typeof(SpeedTable), new PropertyMetadata(Visibility.Collapsed));

        public SpeedTableViewModel Vm { get; private set; }

        public SpeedTable() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new SpeedTableViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
        }

        public void ShowOrHideOverClock(bool isShow) {
            if (isShow) {
                this.IsOverClockVisible = Visibility.Visible;
                this.MenuItemShowOverClock.Visibility = Visibility.Collapsed;
                this.MenuItemHideOverClock.Visibility = Visibility.Visible;
            }
            else {
                this.IsOverClockVisible = Visibility.Collapsed;
                this.MenuItemShowOverClock.Visibility = Visibility.Visible;
                this.MenuItemHideOverClock.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowOrHidePCIE(bool isShow) {
            if (isShow) {
                this.Vm.IsPCIEVisible = Visibility.Visible;
                this.MenuItemShowPCIE.Visibility = Visibility.Collapsed;
                this.MenuItemHidePCIE.Visibility = Visibility.Visible;
            }
            else {
                this.Vm.IsPCIEVisible = Visibility.Collapsed;
                this.MenuItemShowPCIE.Visibility = Visibility.Visible;
                this.MenuItemHidePCIE.Visibility = Visibility.Collapsed;
            }
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }

        private void MenuItemShowOverClock_Click(object sender, RoutedEventArgs e) {
            ShowOrHideOverClock(true);
        }

        private void MenuItemHideOverClock_Click(object sender, RoutedEventArgs e) {
            ShowOrHideOverClock(false);
        }

        private void MenuItemShowPCIE_Click(object sender, RoutedEventArgs e) {
            ShowOrHidePCIE(true);
        }

        private void MenuItemHidePCIE_Click(object sender, RoutedEventArgs e) {
            ShowOrHidePCIE(false);
        }
    }
}
