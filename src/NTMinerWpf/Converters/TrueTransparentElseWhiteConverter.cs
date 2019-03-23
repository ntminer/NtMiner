using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NTMiner.Converters {
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class TrueTransparentElseWhiteConverter : IValueConverter {
        private static readonly SolidColorBrush s_transparent = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush s_white = new SolidColorBrush(Colors.White);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return s_transparent;
            }
            return s_white;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
