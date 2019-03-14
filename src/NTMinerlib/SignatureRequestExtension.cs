using System;

namespace NTMiner {
    public static class SignatureRequestExtension {
        public static bool IsValid<TResponse>(this ISignatureRequest request, Func<string, IUser> getUser, out TResponse response) where TResponse : ResponseBase, new() {
            return IsValid(request, getUser, out _, out response);
        }

        public static bool IsValid<TResponse>(this ISignatureRequest request, Func<string, IUser> getUser, out IUser user, out TResponse response) where TResponse : ResponseBase, new() {
            if (string.IsNullOrEmpty(request.LoginName)) {
                response = ResponseBase.InvalidInput<TResponse>(request.MessageId, "登录名不能为空");
                user = null;
                return false;
            }
            user = getUser.Invoke(request.LoginName);
            if (user == null) {
                string message = "登录名不存在";
                if (request.LoginName == "admin") {
                    message = "第一次使用，请先设置密码";
                }
                response = ResponseBase.NotExist<TResponse>(request.MessageId, message);
                return false;
            }
            if (!request.Timestamp.IsInTime()) {
                response = ResponseBase.Expired<TResponse>(request.MessageId);
                return false;
            }
            if (request.Sign != request.GetSign(user.Password)) {
                response = ResponseBase.Forbidden<TResponse>(request.MessageId, "用户名或密码错误");
                return false;
            }
            response = null;
            return true;
        }
    }
}
