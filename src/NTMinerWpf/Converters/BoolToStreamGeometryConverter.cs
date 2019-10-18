using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NTMiner.Converters {
    [ValueConversion(typeof(bool), typeof(StreamGeometry))]
    public class BoolToStreamGeometryConverter : IValueConverter {
        private static readonly StreamGeometry Icon_CheckedBox = (StreamGeometry)Application.Current.Resources["Icon_CheckedBox"];
        private static readonly StreamGeometry Icon_UnCheckedBox = (StreamGeometry)Application.Current.Resources["Icon_UnCheckedBox"];
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b && b) {
                return Icon_CheckedBox;
            }
            return Icon_UnCheckedBox;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
