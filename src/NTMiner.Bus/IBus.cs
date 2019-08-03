
namespace NTMiner.Bus {
    using System.Collections.Generic;

    public interface IBus {
        void Publish<TMessage>(TMessage message);

        void Publish<TMessage>(IEnumerable<TMessage> messages);

        void Commit();

        void Clear();
    }
}
