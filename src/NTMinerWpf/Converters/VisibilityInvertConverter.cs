using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    public class VisibilityInvertConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Collapsed) {
                return Visibility.Visible;
            }
            else {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
