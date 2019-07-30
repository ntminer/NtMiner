using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public interface IGetSignData {
        StringBuilder GetSignData();
    }

    public class ManualSignAttribute : Attribute {
        public ManualSignAttribute() { }
    }

    public static class GetSignDataExtension {
        private static Dictionary<Type, PropertyInfo[]> _propertyInfos;
        private static PropertyInfo[] GetPropertyInfos(Type type) {
            if (_propertyInfos == null) {
                _propertyInfos = new Dictionary<Type, PropertyInfo[]>();
            }
            if (!_propertyInfos.ContainsKey(type)) {
                _propertyInfos.Add(type, type.GetProperties().Where(a => a.CanRead && a.CanWrite && a.GetCustomAttributes(typeof(ManualSignAttribute), inherit: false).Length == 0).ToArray());
            }
            return _propertyInfos[type];
        }

        public static StringBuilder BuildSign(this IGetSignData obj) {
            var propertyInfos = GetPropertyInfos(obj.GetType());
            StringBuilder sb = new StringBuilder();
            foreach (var propertyInfo in propertyInfos) {
                sb.Append(propertyInfo.Name).Append(propertyInfo.GetValue(obj, null));
            }
            return sb;
        }
    }
}
