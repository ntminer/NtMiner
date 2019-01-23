using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NTMiner.Converters {
    [ValueConversion(typeof(ConsoleColor), typeof(Color))]
    public class ConsoleColorToMediaColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is ConsoleColor c) {
                return c.ToMediaColor();
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Color c) {
                return c.ToConsoleColor();
            }
            return ConsoleColor.White;
        }
    }
}