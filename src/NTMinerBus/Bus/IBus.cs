
namespace NTMiner.Bus {
    using System.Collections.Generic;

    /// <summary>
    /// 电车
    /// </summary>
    public interface IBus {
        /// <summary>
        /// 上车
        /// </summary>
        /// <typeparam name="TMessage">乘客</typeparam>
        /// <param name="message"></param>
        void Publish<TMessage>(TMessage message);

        /// <summary>
        /// 上车
        /// </summary>
        /// <typeparam name="TMessage">乘客集</typeparam>
        /// <param name="messages"></param>
        void Publish<TMessage>(IEnumerable<TMessage> messages);

        /// <summary>
        /// 发车
        /// </summary>
        void Commit();
    }
}
