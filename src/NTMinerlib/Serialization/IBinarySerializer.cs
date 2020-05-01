namespace NTMiner.Serialization {
    public interface IBinarySerializer {
        byte[] Serialize<TObject>(TObject obj);
        TObject Deserialize<TObject>(byte[] data);
        bool IsGZipped(byte[] data);
    }
}
