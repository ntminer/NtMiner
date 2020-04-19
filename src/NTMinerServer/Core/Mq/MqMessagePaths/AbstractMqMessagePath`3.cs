using NTMiner.Hub;
using System;

namespace NTMiner.Core.Mq.MqMessagePaths {
    /// <summary>
    /// 三件事发生后就绪
    /// </summary>
    /// <typeparam name="TEvent1"></typeparam>
    /// <typeparam name="TEvent2"></typeparam>
    /// <typeparam name="TEvent3"></typeparam>
    public abstract class AbstractMqMessagePath<TEvent1, TEvent2, TEvent3> : AbstractMqMessagePath, IMqMessagePath 
        where TEvent1 : IEvent 
        where TEvent2 : IEvent
        where TEvent3 : IEvent {
        private bool _isEvent1Happended = false;
        private bool _isEvent2Happended = false;
        private bool _isEvent3Happended = false;
        public override bool IsReadyToBuild {
            get {
                return _isEvent1Happended && _isEvent2Happended && _isEvent3Happended;
            }
        }

        protected AbstractMqMessagePath(string queue) : base(queue) {
            VirtualRoot.AddOnecePath<TEvent1>($"{typeof(TEvent1).Name}事件已经发生，可以订阅对应的Mq了", LogEnum.UserConsole, action: message => {
                _isEvent1Happended = true;
            }, PathId.Empty, this.GetType());
            VirtualRoot.AddOnecePath<TEvent2>($"{typeof(TEvent2).Name}事件已经发生，可以订阅对应的Mq了", LogEnum.UserConsole, action: message => {
                _isEvent2Happended = true;
            }, PathId.Empty, this.GetType());
            VirtualRoot.AddOnecePath<TEvent2>($"{typeof(TEvent3).Name}事件已经发生，可以订阅对应的Mq了", LogEnum.UserConsole, action: message => {
                _isEvent3Happended = true;
            }, PathId.Empty, this.GetType());
        }
    }
}
