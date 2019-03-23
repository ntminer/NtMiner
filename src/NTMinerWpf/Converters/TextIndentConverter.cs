using System;
using System.Globalization;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(string), typeof(string))]
    public class TextIndentConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                return string.Empty;
            }
            return $"  {value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
