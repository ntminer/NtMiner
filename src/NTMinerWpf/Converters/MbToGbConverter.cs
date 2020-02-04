using System;
using System.Globalization;
using System.Windows.Data;

namespace NTMiner.Converters {
    [ValueConversion(typeof(double), typeof(string))]
    public class MbToGbConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            double d = System.Convert.ToDouble(value);
            return (d / 1024).ToString("f1") + " GB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}