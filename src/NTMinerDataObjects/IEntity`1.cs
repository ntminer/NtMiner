namespace NTMiner {
    public interface IEntity : ICanUpdateByReflection { }

    public interface IEntity<T> : IEntity {
        // 实体的标识字段的命名不一定都叫Id，有些对象可能使用业务上取值唯一的字段作为标识，所以这里不使用属性进行统一命名。
        T GetId();
    }

    public interface IDbEntity<T> : IEntity<T> {
    }
}
