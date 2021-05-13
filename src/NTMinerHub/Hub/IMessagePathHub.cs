using System;
using System.Collections.Generic;

namespace NTMiner.Hub {
    /// <summary>
    /// 集线器，集消息路径器。也可以叫它MessagePathSet。
    /// </summary>
    public interface IMessagePathHub {
        void Route<TMessage>(TMessage message) where TMessage : IMessage;

        /// <summary>
        /// 添加消息路径
        /// </summary>
        /// <typeparam name="TMessage">该消息路径可通行的消息的类型。</typeparam>
        /// <param name="location"><see cref="IMessagePathId.Location"/></param>
        /// <param name="description"><see cref="IMessagePathId.Description"/></param>
        /// <param name="logType"><see cref="IMessagePathId.LogType"/></param>
        /// <param name="pathId"><see cref="IMessagePathId.PathId"/></param>
        /// <param name="priority"><see cref="IMessagePathId.Priority"/></param>
        /// <param name="action">真正的下层的运行时层次的路径，CPU行走的路径，CPU行走在运行时的基本类型空间中，基本类型空间是比如int、bool、string等。</param>
        /// <param name="viaTimesLimit"><see cref="IMessagePathId.ViaTimesLimit"/></param>
        /// <returns></returns>
        IMessagePathId AddPath<TMessage>(
            string location, 
            string description, 
            LogEnum logType, 
            PathId pathId, 
            PathPriority priority, 
            Action<TMessage> action, 
            int viaTimesLimit = -1);

        void RemovePath(IMessagePathId pathId);

        IEnumerable<IMessagePathId> GetAllPaths();

        event Action<IMessagePathId> PathAdded;
        event Action<IMessagePathId> PathRemoved;
    }
}
