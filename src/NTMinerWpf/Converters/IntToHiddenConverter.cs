using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntToHiddenConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is int i && i > 0) {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}