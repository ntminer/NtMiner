using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class KernelGrid : UserControl {
        public KernelsWindowViewModel Vm {
            get {
                return (KernelsWindowViewModel)this.DataContext;
            }
        }

        public KernelGrid() {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<KernelViewModel>(sender, e);
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset - CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftCoin.IsEnabled = offset > 0;
            ButtonRightCoin.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset + CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftCoin.IsEnabled = offset > 0;
            ButtonRightCoin.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void CoinsScrollView_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        private void ButtonLeftBrand_Click(object sender, RoutedEventArgs e) {
            double offset = BrandsScrollView.ContentHorizontalOffset - BrandsScrollView.ViewportWidth;
            BrandsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftBrand.IsEnabled = offset > 0;
            ButtonRightBrand.IsEnabled = offset < BrandsScrollView.ScrollableWidth;
        }

        private void ButtonRightBrand_Click(object sender, RoutedEventArgs e) {
            double offset = BrandsScrollView.ContentHorizontalOffset + BrandsScrollView.ViewportWidth;
            BrandsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeftBrand.IsEnabled = offset > 0;
            ButtonRightBrand.IsEnabled = offset < BrandsScrollView.ScrollableWidth;
        }

        private void BrandsScrollView_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

    }
}
