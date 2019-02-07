using System;
using System.Web;

namespace NTMiner {
    public static class HttpContextExtension {
        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp(this HttpContext httpContext) {
            string userIP = "未获取用户IP";

            try {
                if (httpContext == null
                 || httpContext.Request == null
                 || httpContext.Request.ServerVariables == null) {
                    return "";
                }

                string customerIP = "";

                //CDN加速后取到的IP simone 090805
                customerIP = httpContext.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(customerIP)) {
                    return customerIP;
                }

                customerIP = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!String.IsNullOrEmpty(customerIP)) {
                    return customerIP;
                }

                if (httpContext.Request.ServerVariables["HTTP_VIA"] != null) {
                    customerIP = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (customerIP == null) {
                        customerIP = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else {
                    customerIP = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(customerIP, "unknown", true) == 0 || String.IsNullOrEmpty(customerIP)) {
                    return httpContext.Request.UserHostAddress;
                }
                return customerIP;
            }
            catch { }

            return userIP;

        }
    }
}