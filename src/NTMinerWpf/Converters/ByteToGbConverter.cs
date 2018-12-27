using System;
using System.Globalization;
using System.Windows.Data;

namespace NTMiner.Converters {
    public class ByteToGbConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            long l = (long)value;
            const long g = 1024 * 1024 * 1024;
            return (l / g).ToString() + " GB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}