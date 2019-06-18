
namespace NTMiner {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public class EnumItem<T> {
        public EnumItem(string name, string description, T value) {
            this.Name = name;
            this.Description = description;
            this.Value = value;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public T Value { get; private set; }
    }

    public static class EnumExtension {
        public static string GetName<T>(this T value) where T : struct {
            return EnumDic<T>.Instance[value];
        }

        public static string GetDescription<T>(this T value) where T : struct {
            return EnumDic<T>.Instance.GetDescription(value);
        }

        public static bool TryParse<T>(this string name, out T value) where T : struct {
            if (name != null) {
                return EnumDic<T>.Instance.TryGetValue(name, out value);
            }
            value = default(T);
            return false;
        }

        public static EnumItem<T> GetEnumItem<T>(this T enumValue) where T : struct {
            return EnumDic<T>.Instance.GetEnumItem(enumValue);
        }

        public static IEnumerable<EnumItem<T>> GetEnumItems<T>(this T enumValue) where T : struct {
            return EnumDic<T>.Instance.EnumItems;
        }

        private class EnumDic<T>
            where T : struct {
            public static readonly EnumDic<T> Instance = new EnumDic<T>();

            private readonly Dictionary<ValueType, string> _dicByValue = new Dictionary<ValueType, string>();
            private readonly Dictionary<string, T> _dicByString = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            private readonly Dictionary<ValueType, string> _descriptionByValue = new Dictionary<ValueType, string>();
            private readonly Dictionary<ValueType, EnumItem<T>> _enumItems = new Dictionary<ValueType, EnumItem<T>>();

            private EnumDic() {
                if (typeof(Enum) != typeof(T).BaseType) {
                    throw new NTMinerException(typeof(T).FullName + " must be enum type");
                }
                Type enumType = typeof(T);
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType) as IEnumerable;
                var i = 0;
                foreach (var item in values) {
                    string description = GetDescriptionByName((T)item);
                    _enumItems.Add((T)item, new EnumItem<T>(names[i], description, (T)item));
                    _dicByValue.Add((ValueType)item, names[i]);
                    _dicByString.Add(names[i], (T)item);
                    _descriptionByValue.Add((ValueType)item, description);
                    i++;
                }
            }

            public IEnumerable<EnumItem<T>> EnumItems {
                get {
                    return _enumItems.Values;
                }
            }

            public EnumItem<T> GetEnumItem(T value) {
                return _enumItems[value];
            }

            public string this[T value] {
                get {
                    return _dicByValue[value];
                }
            }

            public bool TryGetValue(string name, out T value) {
                return _dicByString.TryGetValue(name, out value);
            }

            public string this[ValueType value] {
                get {
                    if (!_dicByValue.ContainsKey(value)) {
                        throw new NTMinerException("invalid enum value:" + value.ToString());
                    }

                    return _dicByValue[value];
                }
            }

            public string GetDescription(ValueType value) {
                return _descriptionByValue[value];
            }

            #region private methods
            private static string GetDescriptionByName(T item) {
                FieldInfo fi = item.GetType().GetField(item.ToString());

                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0) {
                    return attributes[0].Description;
                }
                else {
                    return item.ToString();
                }
            }
            #endregion
        }
    }
}
