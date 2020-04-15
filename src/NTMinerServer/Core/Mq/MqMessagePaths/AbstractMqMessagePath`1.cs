using NTMiner.Hub;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    /// <summary>
    /// 一件事发生后就绪
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class AbstractMqMessagePath<TEvent> : AbstractMqMessagePath, IMqMessagePath where TEvent : IEvent {
        private bool _isEventHappended = false;
        public override bool IsReadyToBuild {
            get { return _isEventHappended; }
        }

        protected AbstractMqMessagePath(string queue) : base(queue) {
            VirtualRoot.AddOnecePath<TEvent>($"{typeof(TEvent).Name}事件已经发生，可以订阅对应的Mq了", LogEnum.UserConsole, action: message => {
                _isEventHappended = true;
            }, PathId.Empty, this.GetType());
        }
    }
}
