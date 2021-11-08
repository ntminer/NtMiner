using NTMiner.User;

namespace NTMiner.Core {
    /// <summary>
    /// 只读用户集，WsServer只需要读用户集不需要写用户集所以有了只读用户集。
    /// </summary>
    public interface IReadOnlyUserSet : IRedisLazySet {
        /// <summary>
        /// 根据LoginName或Email或Mobile查询用户
        /// </summary>
        /// <param name="userId">可能是LoginName或Email或Mobile</param>
        /// <returns>用户不存在时返回null</returns>
        UserData GetUser(UserId userId);
    }
}
