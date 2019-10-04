namespace NTMiner.Bus {
    /// <summary>
    /// 处理器
    /// </summary>
    public interface IMessageHandler {
        IHandlerId HandlerId { get; }
    }
}
