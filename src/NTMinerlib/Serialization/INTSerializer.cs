namespace NTMiner.Serialization {
    public interface INTSerializer {
        string Serialize<TObject>(TObject obj);

        TObject Deserialize<TObject>(string json);
    }
}
