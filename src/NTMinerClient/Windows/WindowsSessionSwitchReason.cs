using System.ComponentModel;

namespace NTMiner.Windows {
    /// <summary>
    /// 该类型纯粹是为了在界面上展示翻译后的内容
    /// </summary>
    public enum WindowsSessionSwitchReason {
        [Description("会话已与控制台建立连接")]
        ConsoleConnect = 1,
        [Description("会话已与控制台断开连接")]
        ConsoleDisconnect = 2,
        [Description("会话已与远程连接建立连接")]
        RemoteConnect = 3,
        [Description("会话已与远程连接断开连接")]
        RemoteDisconnect = 4,
        [Description("用户已登录到会话")]
        SessionLogon = 5,
        [Description("用户已从会话注销")]
        SessionLogoff = 6,
        [Description("会话已被锁定")]
        SessionLock = 7,
        [Description("会话已被解除锁定")]
        SessionUnlock = 8,
        [Description("会话已将其状态更改为远程控制状态或从远程控制状态更改为当前的状态")]
        SessionRemoteControl = 9
    }
}
