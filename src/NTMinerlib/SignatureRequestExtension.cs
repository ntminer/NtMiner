using System;

namespace NTMiner {
    public static class SignatureRequestExtension {
        public static bool IsValid<TResponse>(this ISignatureRequest request, Func<string, IUser> getUser, out TResponse response) where TResponse : ResponseBase, new() {
            IUser user;
            return IsValid(request, getUser, out user, out response);
        }

        public static bool IsValid<TResponse>(this ISignatureRequest request, Func<string, IUser> getUser, out IUser user, out TResponse response) where TResponse : ResponseBase, new() {
            if (string.IsNullOrEmpty(request.LoginName)) {
                response = ResponseBase.InvalidInput<TResponse>(request.MessageId, "登录名不能为空");
                user = null;
                return false;
            }
            user = getUser.Invoke(request.LoginName);
            if (user == null) {
                response = ResponseBase.Forbidden<TResponse>(request.MessageId, "登录名不存在");
                return false;
            }
            if (!request.Timestamp.IsInTime()) {
                response = ResponseBase.Expired<TResponse>(request.MessageId);
                return false;
            }
            if (request.Sign != request.GetSign(user.Password)) {
                response = ResponseBase.Forbidden<TResponse>(request.MessageId, "签名验证未通过");
                return false;
            }
            response = null;
            return true;
        }
    }
}
