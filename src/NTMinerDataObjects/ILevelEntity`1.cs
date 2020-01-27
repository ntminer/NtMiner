namespace NTMiner {
    /// <summary>
    /// DataLevel字段不需要存储，因为数据记录所处的位置已经蕴含了DataLevel，所以这里使用显式的
    /// Get和Set方法而不使用公开的属性，从而避免为了避免持久化DataLevel字段而标记BsonIgnore属性。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILevelEntity<T> : IDbEntity<T> {
        DataLevel GetDataLevel();

        void SetDataLevel(DataLevel dataLevel);
    }
}
