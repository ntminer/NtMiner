namespace NTMiner.Serialization {
    public interface IBinarySerializer {
        /// <summary>
        /// 注意：不要对值为object类型的字典序列化或反序列化，应使用强类型。
        /// </summary>
        byte[] Serialize<TObject>(TObject obj);
        /// <summary>
        /// 注意：不要对值为object类型的字典序列化或反序列化，应使用强类型。
        /// </summary>
        TObject Deserialize<TObject>(byte[] data);
        bool IsGZipped(byte[] data);
    }
}
