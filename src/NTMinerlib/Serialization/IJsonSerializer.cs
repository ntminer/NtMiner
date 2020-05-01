namespace NTMiner.Serialization {
    public interface IJsonSerializer {
        string Serialize<TObject>(TObject obj);

        /// <summary>
        /// 不会抛出异常，如果格式不正确返回default
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        TObject Deserialize<TObject>(string json);
    }
}
