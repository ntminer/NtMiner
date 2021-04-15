using NTMiner.Hub;

namespace NTMiner.Core.Mq.MqMessagePaths {
    /// <summary>
    /// 两件事发生后就绪
    /// </summary>
    /// <typeparam name="TEvent1"></typeparam>
    /// <typeparam name="TEvent2"></typeparam>
    public abstract class AbstractMqMessagePath<TEvent1, TEvent2> : AbstractMqMessagePath, IMqMessagePath 
        where TEvent1 : IEvent 
        where TEvent2 : IEvent {
        private bool _isEvent1Happended = false;
        private bool _isEvent2Happended = false;
        public override bool IsReadyToBuild {
            get {
                return _isEvent1Happended && _isEvent2Happended;
            }
        }

        protected AbstractMqMessagePath(string queue) : base(queue) {
            VirtualRoot.BuildOnecePath<TEvent1>($"{typeof(TEvent1).Name}事件已经发生，可以订阅对应的Mq了", LogEnum.UserConsole, PathId.Empty, this.GetType(), PathPriority.Normal, path: message => {
                _isEvent1Happended = true;
            });
            VirtualRoot.BuildOnecePath<TEvent2>($"{typeof(TEvent2).Name}事件已经发生，可以订阅对应的Mq了", LogEnum.UserConsole, PathId.Empty, this.GetType(), PathPriority.Normal, path: message => {
                _isEvent2Happended = true;
            });
        }
    }
}
