namespace NTMiner.Serialization {
    public interface IObjectSerializer {
        string Serialize<TObject>(TObject obj);

        TObject Deserialize<TObject>(string json);
    }
}
