using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class SignatureRequestExtension {
        private static readonly bool _isInnerIpEnabled = Environment.CommandLine.Contains("--enableInnerIp");

        public static bool IsValid<TResponse>(this ISignatureRequest request, IUser user, string sign, DateTime timestamp, string clientIp, out TResponse response) where TResponse : ResponseBase, new() {
            if (clientIp == "localhost" || clientIp == "127.0.0.1") {
                response = null;
                return true;
            }
            if (_isInnerIpEnabled && Ip.Util.IsInnerIp(clientIp)) {
                response = null;
                return true;
            }
            if (user == null) {
                string message = "用户不存在";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            else if (user.LoginName == "admin" && string.IsNullOrEmpty(user.Password)) {
                string message = "第一次使用，请先设置密码";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            if (!timestamp.IsInTime()) {
                response = ResponseBase.Expired<TResponse>();
                return false;
            }
            if (sign != request.GetSign(user.LoginName, user.Password, timestamp)) {
                string message = "用户名或密码错误";
                response = ResponseBase.Forbidden<TResponse>(message);
                return false;
            }
            response = null;
            return true;
        }

        public static string GetSign(this ISignatureRequest request, string loginName, string password, DateTime timestamp) {
            var sb = request.GetSignData();
            sb.Append("LoginName").Append(loginName).Append("Password").Append(password).Append("Timestamp").Append(timestamp);
            return HashUtil.Sha1(sb.ToString());
        }

        public static Dictionary<string, string> ToQuery(this ISignatureRequest request, string loginName, string password) {
            return new Dictionary<string, string> {
                    {"LoginName", loginName },
                    {"Sign", request.GetSign(loginName, password, DateTime.Now) }
                };
        }
    }
}
