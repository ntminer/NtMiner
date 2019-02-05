using System;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class EntityExtensions {
        public static T Update<T, TInput>(this T entity, TInput input) where T : class, IEntity<Guid> {
            if (entity == null) {
                return entity;
            }
            if (input == null) {
                return entity;
            }
            if (ReferenceEquals(entity, input)) {
                return entity;
            }
            Type entityType = entity.GetType();
            Type inputType = input.GetType();
            // 写被写对象的可写属性，且可写属性的set访问器必须数public的
            foreach (PropertyInfo entityProperty in entityType.GetProperties().Where(a => a.CanWrite)) {
                MethodInfo setMethodInfo = entityProperty.GetSetMethod(nonPublic: false);
                if (setMethodInfo == null) {
                    continue;
                }
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
