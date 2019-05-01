namespace NTMiner.Bus {
    public interface IDelegateHandler {
        bool IsPaused { get; set; }
        IHandlerId HandlerId { get; }
    }
}
