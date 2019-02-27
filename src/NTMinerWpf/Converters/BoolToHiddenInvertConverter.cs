using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToHiddenInvertConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return ConvertFun(Visibility.Hidden, parameter);
            }
            return ConvertFun(Visibility.Visible, parameter);
        }

        public object ConvertFun(Visibility visibility, object parameter) {
            if (parameter is string p) {
                if (visibility == Visibility.Visible) {
                    return Visibility.Hidden;
                }
                return Visibility.Visible;
            }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != null && value.Equals(false) ? parameter : Binding.DoNothing;
        }
    }
}
