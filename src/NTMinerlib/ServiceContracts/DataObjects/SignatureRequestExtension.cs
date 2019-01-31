namespace NTMiner.ServiceContracts.DataObjects {
    public static class SignatureRequestExtension {
        public static bool IsValid<TResponse>(this ISignatureRequest request, IUserSet userSet, out TResponse response) where TResponse : ResponseBase, new() {
            if (string.IsNullOrEmpty(request.LoginName)) {
                response = ResponseBase.InvalidInput<TResponse>(request.MessageId, "登录名不能为空");
                return false;
            }
            if (!userSet.TryGetKey(request.LoginName, out IUser key)) {
                response = ResponseBase.Forbidden<TResponse>(request.MessageId, "登录名不存在");
                return false;
            }
            if (!request.Timestamp.IsInTime()) {
                response = ResponseBase.Expired<TResponse>(request.MessageId);
                return false;
            }
            if (request.Sign != request.GetSign(key.Password)) {
                response = ResponseBase.Forbidden<TResponse>(request.MessageId, "签名验证未通过");
                return false;
            }
            response = null;
            return true;
        }
    }
}
