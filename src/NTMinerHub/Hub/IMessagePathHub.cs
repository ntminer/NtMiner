using System;
using System.Collections.Generic;

namespace NTMiner.Hub {
    /// <summary>
    /// 集线器，集消息路径器。也可以叫它MessagePathSet。
    /// </summary>
    public interface IMessagePathHub {
        void Route<TMessage>(TMessage message) where TMessage : IMessage;

        void AddPath<TMessage>(MessagePath<TMessage> path);

        void RemovePath(IMessagePathId pathId);

        IEnumerable<IMessagePathId> GetAllPaths();

        event Action<IMessagePathId> PathAdded;
        event Action<IMessagePathId> PathRemoved;
    }
}
