using System;
using System.Collections.Generic;

namespace NTMiner.Hub {
    /// <summary>
    /// 集线器，集消息路径器。也可以叫它MessagePathSet。
    /// </summary>
    public interface IMessageHub {
        void Route<TMessage>(TMessage message) where TMessage : IMessage;

        void AddMessagePath<TMessage>(MessagePath<TMessage> handler);

        void RemoveMessagePath(IMessagePathId handlerId);

        IEnumerable<IMessagePathId> GetAllPaths();

        event Action<IMessagePathId> MessagePathAdded;
        event Action<IMessagePathId> MessagePathRemoved;
    }
}
