using System;
using System.Net;

namespace NTMiner.FileETag {
    public static class ETagClient {
        public static void HeadETag(string fileUrl, Action<string> callback) {
            try {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(fileUrl));
                webRequest.Method = "HEAD";
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                string etag = response.GetResponseHeader("ETag");
                callback?.Invoke(etag);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                callback?.Invoke(string.Empty);
            }
        }
    }
}
