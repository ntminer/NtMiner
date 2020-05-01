namespace NTMiner.Serialization {
    public interface IBinarySerializer {
        byte[] Serialize<TObject>(TObject obj);
        TObject Deserialize<TObject>(byte[] stream);
    }
}
