namespace NTMiner {
    public interface ILevelEntity<T> : IDbEntity<T> {
        DataLevel DataLevel { get; }

        void SetDataLevel(DataLevel dataLevel);
    }
}
