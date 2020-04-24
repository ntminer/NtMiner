using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public interface ISignableData {
        StringBuilder GetSignData();
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ManualSignAttribute : Attribute {
        public ManualSignAttribute() { }
    }

    internal static class SignableDataExtension {
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyInfos = new Dictionary<Type, PropertyInfo[]>();
        private static readonly object _locker = new object();
        private static PropertyInfo[] GetPropertyInfos(Type type) {
            if (!_propertyInfos.TryGetValue(type, out PropertyInfo[] properties)) {
                lock (_locker) {
                    if (!_propertyInfos.TryGetValue(type, out properties)) {
                        // 反射出的属性集合的顺序是和静态源代码声明的顺序一致的，但静态源代码的顺序可能会被调整，所以当基于反射的数据签名时必须考虑到排序。
                        properties = type.GetProperties().Where(a => a.CanRead && a.CanWrite && a.GetCustomAttributes(typeof(ManualSignAttribute), inherit: false).Length == 0).OrderBy(a => a.Name).ToArray();
                        _propertyInfos.Add(type, properties);
                    }
                }
            }
            return properties;
        }

        internal static StringBuilder BuildSign(this ISignableData data) {
            var propertyInfos = GetPropertyInfos(data.GetType());
            StringBuilder sb = new StringBuilder();
            foreach (var propertyInfo in propertyInfos) {
                sb.Append(propertyInfo.Name).Append(propertyInfo.GetValue(data, null));
            }
            return sb;
        }
    }
}
