using Qiniu.Storage;
using Qiniu.Util;

namespace NTMiner.NTMinerFileUrlGenerater.Impl {
    public class QiniuNTMinerKodoFileUrlGenerater : INTMinerFileUrlGenerater {
        private readonly Mac _mac;
        /// <summary>
        /// 阿里云OSS比七牛Kodo贵一半。
        /// </summary>
        public QiniuNTMinerKodoFileUrlGenerater() {
            _mac = new Mac(ServerRoot.HostConfig.KodoAccessKey, ServerRoot.HostConfig.KodoSecretKey);
        }

        public string GeneratePresignedUrl(string bucketName, string key) {
            // 注意：Qiniu.dll是github上下载源码编译的，因为Qiniu依赖的Newtonsoft.Json版本不对
            string domain = $"http://{bucketName}.clouddn.com";
            int expireInSeconds = 600;
            string privateUrl = DownloadManager.CreatePrivateUrl(_mac, domain, key, expireInSeconds);
            return privateUrl;
        }
    }
}
