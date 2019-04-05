namespace NTMiner.Core.Gpus.Impl.Nvidia {
    public class CudaVersion : ICudaVersion {
        public CudaVersion() { }

        public CudaVersion(ICudaVersion data) {
            this.Version = data.Version;
            this.Description = data.Description;
            this.MinDriverVersion = data.MinDriverVersion;
        }

        [LiteDB.BsonId]
        public string Version { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 大于等于该驱动版本号
        /// </summary>
        public double MinDriverVersion { get; set; }

        public string GetId() {
            return this.Version;
        }
    }
}
