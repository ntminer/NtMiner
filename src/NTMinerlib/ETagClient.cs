using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NTMiner {
    public static class ETagClient {
        public static void HeadETagAsync(string fileUrl, Action<string> callback) {
            Task.Factory.StartNew(() => {
                try {
                    Global.Logger.InfoDebugLine("HeadETagAsync " + fileUrl);
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(fileUrl));
                    webRequest.Timeout = 300 * 1000;
                    webRequest.Method = "HEAD";
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    string etag = response.GetResponseHeader("ETag").Trim('"');
                    callback?.Invoke(etag);
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(string.Empty);
                }
            });
        }

        public static void GetFileAsync(string fileUrl, Action<string, byte[]> callback) {
            Task.Factory.StartNew(() => {
                try {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(fileUrl));
                    webRequest.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    string etag = response.GetResponseHeader("ETag").Trim('"');
                    using (MemoryStream ms = new MemoryStream())
                    using (Stream stream = response.GetResponseStream()) {
                        byte[] buffer = new byte[1024];
                        int n = stream.Read(buffer, 0, buffer.Length);
                        while (n > 0) {
                            ms.Write(buffer, 0, n);
                            n = stream.Read(buffer, 0, buffer.Length);
                        }
                        byte[] data = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                        callback?.Invoke(etag, data);
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(string.Empty, new byte[0]);
                }
            });
        }
    }
}
