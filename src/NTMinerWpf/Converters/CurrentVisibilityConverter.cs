using MinerClient.Vms;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MinerClient.Converters {
    public class CurrentVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (parameter == null) {
                return Visibility.Collapsed;
            }
            if (MinerProfileViewModel.Current.SelectedMainCoin == value) {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}