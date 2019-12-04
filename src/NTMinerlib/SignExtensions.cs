using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class SignExtensions {
        private static readonly bool _isInnerIpEnabled = Environment.CommandLine.Contains(NTKeyword.EnableInnerIpCmdParameterName);

        public static bool IsValid<TResponse>(this IGetSignData data, IUser user, string sign, ulong timestamp, string clientIp, out TResponse response) where TResponse : ResponseBase, new() {
            if (clientIp == "localhost" || clientIp == "127.0.0.1") {
                response = null;
                return true;
            }
            if (_isInnerIpEnabled && Net.Util.IsInnerIp(clientIp)) {
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
            string mySign = GetSign(data, user.LoginName, user.Password, timestamp);
            if (sign != mySign) {
                string message = "用户名或密码错误";
                response = ResponseBase.Forbidden<TResponse>(message);
                Write.DevDebug($"{message} sign:{sign} mySign:{mySign}");
                return false;
            }
            response = null;
            return true;
        }

        private static string GetSign(IGetSignData data, string loginName, string password, ulong timestamp) {
            var sb = data.GetSignData();
            sb.Append("LoginName").Append(loginName).Append("Password").Append(password).Append("Timestamp").Append(timestamp);
            return HashUtil.Sha1(sb.ToString());
        }

        public static Dictionary<string, string> ToQuery(this IGetSignData data, string loginName, string password) {
            var timestamp = Timestamp.GetTimestamp(DateTime.Now);
            return new Dictionary<string, string> {
                    {"loginName", loginName },
                    {"sign", GetSign(data, loginName, password, timestamp) },
                    {"timestamp", timestamp.ToString() }
                };
        }
    }
}
