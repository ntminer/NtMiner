using System;
using System.Globalization;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolInvertConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
