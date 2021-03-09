using Aliyun.OSS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qiniu.Storage;
using Qiniu.Util;
using System;

namespace NTMiner {
    [TestClass]
    public class OSSTests {
        [TestMethod]
        public void AliGeneratePresignedUriTest() {
            string ossEndpoint = "oss-cn-beijing.aliyuncs.com";
            string ossAccessKeyId = "ATAI4G7BGjsCjRF2ThyDdgBo";// 假的
            string ossAccessKeySecret = "A1VdRfHJ6k8NeklCtHjQlt7MYlRShE";// 假的
            var ossClient= new OssClient(ossEndpoint, ossAccessKeyId, ossAccessKeySecret);
            var req = new GeneratePresignedUriRequest("ntminer", "helloworld.exe", SignHttpMethod.Get) {
                Expiration = DateTime.Now.AddMinutes(10)
            };
            var uri = ossClient.GeneratePresignedUri(req);
            Console.WriteLine(uri.ToString());
            req = new GeneratePresignedUriRequest("ntminer", "helloworld.exe", SignHttpMethod.Get);
            uri = ossClient.GeneratePresignedUri(req);
            Console.WriteLine(uri.ToString());
            TimeSpan ts = req.Expiration - DateTime.Now;
            Console.WriteLine(ts.TotalSeconds);
            // 默认是15分钟
            Assert.IsTrue(ts.TotalSeconds > 14 * 60 && ts.TotalSeconds < 16 * 60);
        }

        [TestMethod]
        public void KodoCreateSignedUrlTest() {
            // 注意：Qiniu.dll是github上下载源码编译的，因为Qiniu依赖的Newtonsoft.Json版本不对
            string kodoAccessKey = "ATAI4G7BGjsCjRF2ThyDdgBo";// 假的
            string kodoSecretKey = "A1VdRfHJ6k8NeklCtHjQlt7MYlRShE";// 假的
            Mac mac = new Mac(kodoAccessKey, kodoSecretKey);
            string domain = "http://aaa.qiniudn.com";
            string key = "hello/world/七牛/test.png";
            string privateUrl = DownloadManager.CreatePrivateUrl(mac, domain, key, 600);
            Console.WriteLine(privateUrl);
        }
    }
}
