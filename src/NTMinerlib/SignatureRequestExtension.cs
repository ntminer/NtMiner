using System;

namespace NTMiner {
    public static class SignatureRequestExtension {
        private static readonly bool _isInnerIpEnabled = Environment.CommandLine.Contains("--enableInnerIp");

        public static bool IsValid<TResponse>(this ISignatureRequest request, Func<string, IUser> getUser, string clientIp, out TResponse response) where TResponse : ResponseBase, new() {
            return IsValid(request, getUser, clientIp, out _, out response);
        }

        public static bool IsValid<TResponse>(this ISignatureRequest request, Func<string, IUser> getUser, string clientIp, out IUser user, out TResponse response) where TResponse : ResponseBase, new() {
            if (clientIp == "localhost" || clientIp == "127.0.0.1") {
                user = null;
                response = null;
                return true;
            }
            if (_isInnerIpEnabled && Ip.Util.IsInnerIp(clientIp)) {
                user = null;
                response = null;
                return true;
            }
            if (string.IsNullOrEmpty(request.LoginName)) {
                response = ResponseBase.InvalidInput<TResponse>("登录名不能为空");
                user = null;
                return false;
            }
            user = getUser.Invoke(request.LoginName);
            if (user == null) {
                string message = "登录名不存在";
                if (request.LoginName == "admin") {
                    message = "第一次使用，请先设置密码";
                }
                Write.DevLine($"{request.LoginName} {message}");
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            if (!request.Timestamp.IsInTime()) {
                Write.DevLine($"过期的请求 {request.Timestamp}");
                response = ResponseBase.Expired<TResponse>();
                return false;
            }
            if (request.Sign != request.GetSign(user.Password)) {
                string message = "用户名或密码错误";
                Write.DevLine($"{request.LoginName} {message}");
                response = ResponseBase.Forbidden<TResponse>(message);
                return false;
            }
            response = null;
            return true;
        }
    }
}
