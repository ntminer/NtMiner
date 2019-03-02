using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NoRecordInvertVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                return Visibility.Collapsed;
            }
            if (value is IEnumerable enumerable) {
                int i = 0;
                foreach (var item in enumerable) {
                    i++;
                    break;
                }
                if (i == 0) {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}