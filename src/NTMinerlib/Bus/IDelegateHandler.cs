namespace NTMiner.Bus {
    public interface IDelegateHandler {
        bool IsEnabled { get; set; }
        IHandlerId HandlerId { get; }
    }
}
