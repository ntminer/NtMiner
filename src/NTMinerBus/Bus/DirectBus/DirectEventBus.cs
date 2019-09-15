
namespace NTMiner.Bus.DirectBus {
    public sealed class DirectEventBus : DirectBus, IEventBus {
        public DirectEventBus(IMessageDispatcher dispatcher) : base(dispatcher) { }
    }
}
