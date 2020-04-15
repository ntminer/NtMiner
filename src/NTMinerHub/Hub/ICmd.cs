namespace NTMiner.Hub {

    /// <summary>
    /// 一种命令只应被一个处理器处理，命令实际上可以设计为不走总线，之
    /// 所以设计为统一走总线只是为了通过将命令类型集中表达以起文档作用。
    /// </summary>
    public interface ICmd : IMessage {
    }
}
