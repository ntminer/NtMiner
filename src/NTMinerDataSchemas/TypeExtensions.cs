using System;

namespace NTMiner {
    public static class TypeExtensions {
        public static bool TryGetAttribute<T>(this Type type, out T attribute) where T : Attribute {
            var objs = type.GetCustomAttributes(typeof(T), inherit: false);
            if (objs != null && objs.Length != 0) {
                attribute = (T)objs[0];
                return true;
            }
            attribute = default(T);
            return false;
        }
    }
}
