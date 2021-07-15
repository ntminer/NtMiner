using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner {
    public static class HtmlUtil {
        public static async Task<string> GetF2poolHtmlAsync() {
            return await Task.Factory.StartNew(() => {
                try {
                    string url = $"https://www.f2pool.com/?t={DateTime.Now.Ticks.ToString()}";
                    if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) {
                        // 没有这一行可能会报错：System.Net.WebException: 请求被中止: 未能创建 SSL/TLS 安全通道
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                    httpWebRequest.Timeout = 30 * 1000;
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Referer= "http://dl.ntminer.top";
                    httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                    httpWebRequest.Headers["accept-encoding"] = "gzip, deflate, br";
                    httpWebRequest.Headers["accept-language"] = "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6";
                    httpWebRequest.Headers["cache-control"] = "max-age=0";
                    httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.67";
                    var response = httpWebRequest.GetResponse();
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
                        return Encoding.UTF8.GetString(data);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    return string.Empty;
                }
            });
        }
    }
}
