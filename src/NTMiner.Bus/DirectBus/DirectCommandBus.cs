
namespace NTMiner.Bus.DirectBus {
    public sealed class DirectCommandBus : DirectBus, ICmdBus {
        public DirectCommandBus(IMessageDispatcher dispatcher) : base(dispatcher) { }
    }
}
