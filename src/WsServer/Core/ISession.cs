using NTMiner.User;
using NTMiner.Ws;
using System;
using WebSocketSharp;

namespace NTMiner.Core {
    public interface ISession {
        /// <summary>
        /// 这是客户端登录时传过来的原始数据。WebSocket框架层收到的是个经过base64编码的json字符串，
        /// 承载在WebSocket框架层的User.Identity.Name处，WsUserName是对该字符串反序列化后得到的对象。
        /// 为什么json经过了base64编码？因为使用的WebSocket框架中会特别对待User.Identity.Name处的
        /// 特殊字符，所以不能直接在这个位置承载json，所以进行base64编码。
        /// </summary>
        IWsUserName WsUserName { get; }
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
        /// 会话的最新活动时间。清洁工线程会依据该值判断是否要清理掉不活跃的连接对应的资源。
        /// </summary>
        DateTime ActiveOn { get; }
        /// <summary>
        /// 这是所使用的WebSocket框架层为当前会话分配的会话标识。
        /// </summary>
        string WsSessionId { get; }
        /// <summary>
        /// 委托给下层的WebSocket框架关闭当前会话。<see cref="WebSocket.CloseAsync(CloseStatusCode, string)"/>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="reason"></param>
        void CloseAsync(CloseStatusCode code, string reason);
        /// <summary>
        /// 委托给下层的WebSocket框架向客户端发送给定的消息，发送的消息使用给定的密码签名，
        /// 其内部根据客户端是否支持二进制消息(<see cref="IWsUserName.IsBinarySupported"/>)
        /// 或委托给<see cref="WebSocket.SendAsync(byte[], Action{bool})"/>
        /// 或委托给<see cref="WebSocket.SendAsync(string, Action{bool})"/>。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="password"></param>
        void SendAsync(WsMessage message, string password);
        /// <summary>
        /// 将ActiveOn修置为当前时间。
        /// </summary>
        void Active();
    }
}
