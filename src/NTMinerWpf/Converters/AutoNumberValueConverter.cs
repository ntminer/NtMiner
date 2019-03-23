using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace NTMiner.Converters {
    class AutoNumberValueConverter : IMultiValueConverter {
        #region IMultiValueConverter 成员

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var item = values[0];
            var items = values[1] as ItemCollection;
            if (items == null) {
                return string.Empty;
            }
            var index = items.IndexOf(item);
            return (index + 1).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            return null;
        }

        #endregion
    }
}
