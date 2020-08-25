using NTMiner.Services.OSS;

namespace NTMiner.Services {
    public class OSSService {
        public readonly AliyunOSSService AliyunOSSService = new AliyunOSSService();

        internal OSSService() { }
    }
}
