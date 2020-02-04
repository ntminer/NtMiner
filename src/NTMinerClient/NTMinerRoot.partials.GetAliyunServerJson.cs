using NTMiner.Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot {
        private static void GetAliyunServerJson(Action<byte[]> callback) {
            string serverJsonFileUrl = $"{OfficialServices.MinerJsonBucket}{EntryAssemblyInfo.ServerJsonFileName}";
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
                        byte[] buffer = new byte[1024];
                        int n = stream.Read(buffer, 0, buffer.Length);
                        while (n > 0) {
                            ms.Write(buffer, 0, n);
                            n = stream.Read(buffer, 0, buffer.Length);
                        }
                        byte[] data = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                        data = ZipDecompress(data);
                        callback?.Invoke(data);
                    }
                    Logger.InfoDebugLine($"下载完成：{fileUrl}");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    callback?.Invoke(new byte[0]);
                }
            });
        }

        private static byte[] ZipDecompress(byte[] zippedData) {
            using (Stream ms = new MemoryStream(zippedData), 
                          compressedzipStream = new GZipStream(ms, CompressionMode.Decompress), 
                          outBuffer = new MemoryStream()) {
                byte[] block = new byte[1024];
                while (true) {
                    int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0) {
                        break;
                    }
                    else {
                        outBuffer.Write(block, 0, bytesRead);
                    }
                }
                compressedzipStream.Close();
                return ((MemoryStream)outBuffer).ToArray();
            }
        }
    }
}
