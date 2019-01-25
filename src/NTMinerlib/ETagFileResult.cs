namespace NTMiner {
    public class ETagFileResult {
        public ETagFileResult(string etag, byte[] data) {
            this.ETag = etag;
            this.Data = data;
        }

        public string ETag { get; private set; }
        public byte[] Data { get; private set; }
    }
}
