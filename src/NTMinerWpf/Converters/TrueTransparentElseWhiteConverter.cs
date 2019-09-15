using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NTMiner.Converters {
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class TrueTransparentElseWhiteConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return Wpf.Util.TransparentBrush;
            }
            return Wpf.Util.WhiteBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
