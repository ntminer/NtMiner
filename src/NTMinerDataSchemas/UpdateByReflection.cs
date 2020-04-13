using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class UpdateByReflection {
        private static readonly Dictionary<Type, PropertyInfo[]> SEntityPropertiesDic = new Dictionary<Type, PropertyInfo[]>();
        public static T Update<T, TInput>(this T entity, TInput input) where T : class, ICanUpdateByReflection {
            if (entity == null) {
                return null;
            }
            if (input == null) {
                return entity;
            }
            if (ReferenceEquals(entity, input)) {
                // 视为编程错误，从而避免发生隐蔽的BUG
                throw new InvalidProgramException();
            }
            Type entityType = entity.GetType();
            Type inputType = input.GetType();
            // 写被写对象的可写属性，且可写属性的set访问器必须数public的
            if (!SEntityPropertiesDic.ContainsKey(entityType)) {
                Type attrubuteType = typeof(IgnoreReflectionSetAttribute);
                SEntityPropertiesDic.Add(entityType, entityType.GetProperties()
                    .Where(a => a.CanWrite
                                && a.GetSetMethod(nonPublic: false) != null
                                && (a.GetCustomAttributes(attrubuteType, inherit: false).Length == 0))
                    .ToArray());
            }
            foreach (PropertyInfo entityProperty in SEntityPropertiesDic[entityType]) {
                PropertyInfo inputProperty = inputType.GetProperty(entityProperty.Name);
                if (inputProperty == null) {
                    continue;
                }
                entityProperty.SetValue(entity, inputProperty.GetValue(input, null), null);
            }

            return entity;
        }
    }
}
