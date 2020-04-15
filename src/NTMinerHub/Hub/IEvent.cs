namespace NTMiner.Hub {
    using System;

    public interface IEvent : IMessage {
        /// <summary>
        /// 事件的目的路径，该事件将会被泵到该路径去，取值PathId.Empty表示不特定标识，只要特定消息类型。
        /// </summary>
        PathId TargetPathId { get; }
        /// <summary>
        /// 事件诞生时的时间戳
        /// </summary>
        DateTime BornOn { get; }
    }
}
