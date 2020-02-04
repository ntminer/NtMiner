using NTMiner.Hub;
using System;

namespace NTMiner {
    [MessageType(description: "关闭窗口")]
    public class CloseWindowCommand : Cmd {
        public CloseWindowCommand(Guid id) : base(id) { }
    }

    [MessageType(description: "打开本机IP管理页")]
    public class ShowLocalIpsCommand : Cmd {
        public ShowLocalIpsCommand() { }
    }
}
