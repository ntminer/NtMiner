namespace NTMiner.Serialization {
    public interface IBinarySerializer {
        /// <summary>
        /// 如果对象序列化为json后的尺寸大于<see cref="NTKeyword.IntK"/>则会GZip压缩。
        /// 注意：不要对值为object类型的字典序列化或反序列化，应使用强类型。
        /// </summary>
        byte[] Serialize<TObject>(TObject obj);
        /// <summary>
        /// 如果字节数组的开头有gzipped特征则过程内部会自动解压。
        /// 注意：不要对值为object类型的字典序列化或反序列化，应使用强类型。
        /// </summary>
        TObject Deserialize<TObject>(byte[] data);
        bool IsGZipped(byte[] data);
    }
}
