using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner.Services.OSS {
    public class AliyunOSSService {
        internal AliyunOSSService() { }

        public void GetAliyunServerLitedb(Action<byte[]> callback) {
            string serverLitedbFileUrl = $"https://minerjson.{NTKeyword.CloudFileDomain}/{NTKeyword.ServerDbFileName}";
            string fileUrl = serverLitedbFileUrl + "?t=" + DateTime.Now.Ticks;
            Task.Factory.StartNew(() => {
                try {
                    var webRequest = WebRequest.Create(fileUrl);
                    webRequest.Timeout = 20 * 1000;
                    webRequest.Method = "GET";
                    var response = webRequest.GetResponse();
                    using (Stream ms = new MemoryStream(), stream = response.GetResponseStream()) {
                        byte[] buffer = new byte[NTKeyword.IntK];
                        int n = stream.Read(buffer, 0, buffer.Length);
                        while (n > 0) {
                            ms.Write(buffer, 0, n);
                            n = stream.Read(buffer, 0, buffer.Length);
                        }
                        byte[] data = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                        callback?.Invoke(data);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(new byte[0]);
                }
            });
        }

        public void GetAliyunServerJson(Action<string> callback) {
            string serverJsonFileUrl = $"https://minerjson.{NTKeyword.CloudFileDomain}/{HomePath.ExportServerJsonFileName}";
            string fileUrl = serverJsonFileUrl + "?t=" + DateTime.Now.Ticks;
            Task.Factory.StartNew(() => {
                try {
                    var webRequest = WebRequest.Create(fileUrl);
                    webRequest.Timeout = 20 * 1000;
                    webRequest.Method = "GET";
                    // 因为有压缩和解压缩，server.json的尺寸已经不是问题
                    webRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    var response = webRequest.GetResponse();
                    using (Stream ms = new MemoryStream(), stream = response.GetResponseStream()) {
                        byte[] buffer = new byte[NTKeyword.IntK];
                        int n = stream.Read(buffer, 0, buffer.Length);
                        while (n > 0) {
                            ms.Write(buffer, 0, n);
                            n = stream.Read(buffer, 0, buffer.Length);
                        }
                        byte[] data = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                        data = RpcRoot.ZipDecompress(data);
                        callback?.Invoke(Encoding.UTF8.GetString(data));
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(string.Empty);
                }
            });
        }
    }
}
