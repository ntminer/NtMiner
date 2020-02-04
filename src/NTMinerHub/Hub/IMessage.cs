
using System;

namespace NTMiner.Hub {
    public interface IMessage {
        Guid MessageId { get; }
    }
}
