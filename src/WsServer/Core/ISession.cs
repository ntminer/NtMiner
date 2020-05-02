using System;

namespace NTMiner.Core {
    public interface ISession {
        /// <summary>
        /// 客户端进程的标识：挖矿端进程的标识或群控客户端进程的标识
        /// </summary>
        /// <remarks>注意这里说的进程不是指的操作系统进程而是指运行中的开源矿工程序</remarks>
        Guid ClientId { get; }
        /// <summary>
        /// 用户的标识，用户的标识是LoginName没有个叫Id的字段。根据登录时的LoginName或Email或Mobile从数据库中查询得到的LoginName。
        /// </summary>
        string LoginName { get; }
        /// <summary>
        /// 客户端进程的版本号
        /// </summary>
        /// <remarks>注意这里说的进程不是指的操作系统进程而是指运行中的开源矿工程序</remarks>
        Version ClientVersion { get; }
        /// <summary>
        /// 会话的最新活动时间
        /// </summary>
        DateTime ActiveOn { get; }
        string WsSessionId { get; }
        void Active();
    }
}
