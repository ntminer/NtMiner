namespace NTMiner {
    public interface IEntity { }

    public interface IEntity<T> : IEntity {
        T GetId();
    }

    public interface IDbEntity<T> : IEntity<T> {
    }
}
