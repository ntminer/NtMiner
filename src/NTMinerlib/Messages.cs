using NTMiner.Bus;
using System;

namespace NTMiner {

    #region abstract
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(TEntity source) {
            this.Id = Guid.NewGuid();
            this.Source = source;
            this.Timestamp = DateTime.Now;
        }
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TEntity Source { get; private set; }
    }

    public abstract class AddEntityCommand<TEntity> : Cmd where TEntity : class, IEntity<Guid> {
        protected AddEntityCommand(TEntity input) {
            if (input == null) {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public TEntity Input { get; private set; }
    }

    public abstract class RemoveEntityCommand : Cmd {
        protected RemoveEntityCommand(Guid entityId) {
            this.EntityId = entityId;
        }

        public Guid EntityId { get; private set; }
    }

    public abstract class UpdateEntityCommand<TEntity> : Cmd where TEntity : class, IEntity<Guid> {
        protected UpdateEntityCommand(TEntity input) {
            if (input == null) {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public TEntity Input { get; private set; }
    }
    #endregion

    [MessageType(messageType: typeof(UpdateHandlerIdCommand), description: "更新处理器日志配置")]
    public class UpdateHandlerIdCommand : UpdateEntityCommand<IHandlerId> {
        public UpdateHandlerIdCommand(IHandlerId input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(HandlerIdAddedEvent), description: "新的处理器标识出现后")]
    public class HandlerIdAddedEvent : DomainEvent<IHandlerId> {
        public HandlerIdAddedEvent(IHandlerId source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(HandlerIdUpdatedEvent), description: "更新了处理器标识信息后")]
    public class HandlerIdUpdatedEvent : DomainEvent<IHandlerId> {
        public HandlerIdUpdatedEvent(IHandlerId source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(HasBoot1SecondEvent), description: "已经启动1秒钟", isCanNoHandler: true)]
    public class HasBoot1SecondEvent : EventBase {
        public readonly int Seconds = 1;
    }

    [MessageType(messageType: typeof(HasBoot2SecondEvent), description: "已经启动2秒钟", isCanNoHandler: true)]
    public class HasBoot2SecondEvent : EventBase {
        public readonly int Seconds = 2;
    }

    [MessageType(messageType: typeof(HasBoot5SecondEvent), description: "已经启动5秒钟", isCanNoHandler: true)]
    public class HasBoot5SecondEvent : EventBase {
        public readonly int Seconds = 5;
    }

    [MessageType(messageType: typeof(HasBoot10SecondEvent), description: "已经启动10秒钟", isCanNoHandler: true)]
    public class HasBoot10SecondEvent : EventBase {
        public readonly int Seconds = 10;
    }

    [MessageType(messageType: typeof(HasBoot20SecondEvent), description: "已经启动20秒钟", isCanNoHandler: true)]
    public class HasBoot20SecondEvent : EventBase {
        public readonly int Seconds = 20;
    }

    [MessageType(messageType: typeof(HasBoot1MinuteEvent), description: "已经启动1分钟", isCanNoHandler: true)]
    public class HasBoot1MinuteEvent : EventBase {
        public readonly int Seconds = 60;
    }

    [MessageType(messageType: typeof(HasBoot2MinuteEvent), description: "已经启动2分钟", isCanNoHandler: true)]
    public class HasBoot2MinuteEvent : EventBase {
        public readonly int Seconds = 120;
    }

    [MessageType(messageType: typeof(HasBoot5MinuteEvent), description: "已经启动5分钟", isCanNoHandler: true)]
    public class HasBoot5MinuteEvent : EventBase {
        public readonly int Seconds = 300;
    }

    [MessageType(messageType: typeof(HasBoot10MinuteEvent), description: "已经启动10分钟", isCanNoHandler: true)]
    public class HasBoot10MinuteEvent : EventBase {
        public readonly int Seconds = 600;
    }

    [MessageType(messageType: typeof(HasBoot20MinuteEvent), description: "已经启动20分钟", isCanNoHandler: true)]
    public class HasBoot20MinuteEvent : EventBase {
        public readonly int Seconds = 1200;
    }

    [MessageType(messageType: typeof(HasBoot50MinuteEvent), description: "已经启动50分钟", isCanNoHandler: true)]
    public class HasBoot50MinuteEvent : EventBase {
        public readonly int Seconds = 3000;
    }

    [MessageType(messageType: typeof(HasBoot100MinuteEvent), description: "已经启动100分钟", isCanNoHandler: true)]
    public class HasBoot100MinuteEvent : EventBase {
        public readonly int Seconds = 6000;
    }

    [MessageType(messageType: typeof(HasBoot24HourEvent), description: "已经启动24小时", isCanNoHandler: true)]
    public class HasBoot24HourEvent : EventBase {
        public readonly int Seconds = 86400;
    }


    [MessageType(messageType: typeof(Per1SecondEvent), description: "每1秒事件", isCanNoHandler: true)]
    public class Per1SecondEvent : EventBase {
        public readonly int Seconds = 1;

        public Per1SecondEvent() { }
    }

    [MessageType(messageType: typeof(Per2SecondEvent), description: "每2秒事件", isCanNoHandler: true)]
    public class Per2SecondEvent : EventBase {
        public readonly int Seconds = 2;

        public Per2SecondEvent() { }
    }

    [MessageType(messageType: typeof(Per5SecondEvent), description: "每5秒事件", isCanNoHandler: true)]
    public class Per5SecondEvent : EventBase {
        public readonly int Seconds = 5;

        public Per5SecondEvent() { }
    }

    [MessageType(messageType: typeof(Per10SecondEvent), description: "每10秒事件", isCanNoHandler: true)]
    public class Per10SecondEvent : EventBase {
        public readonly int Seconds = 10;

        public Per10SecondEvent() { }
    }

    [MessageType(messageType: typeof(Per20SecondEvent), description: "每20秒事件", isCanNoHandler: true)]
    public class Per20SecondEvent : EventBase {
        public readonly int Seconds = 20;

        public Per20SecondEvent() { }
    }

    [MessageType(messageType: typeof(Per1MinuteEvent), description: "每1分钟事件", isCanNoHandler: true)]
    public class Per1MinuteEvent : EventBase {
        public readonly int Seconds = 60;

        public Per1MinuteEvent() { }
    }

    [MessageType(messageType: typeof(Per2MinuteEvent), description: "每2分钟事件", isCanNoHandler: true)]
    public class Per2MinuteEvent : EventBase {
        public readonly int Seconds = 120;

        public Per2MinuteEvent() { }
    }

    [MessageType(messageType: typeof(Per5MinuteEvent), description: "每5分钟事件", isCanNoHandler: true)]
    public class Per5MinuteEvent : EventBase {
        public readonly int Seconds = 300;

        public Per5MinuteEvent() { }
    }

    [MessageType(messageType: typeof(Per10MinuteEvent), description: "每10分钟事件", isCanNoHandler: true)]
    public class Per10MinuteEvent : EventBase {
        public readonly int Seconds = 600;

        public Per10MinuteEvent() { }
    }

    [MessageType(messageType: typeof(Per20MinuteEvent), description: "每20分钟事件", isCanNoHandler: true)]
    public class Per20MinuteEvent : EventBase {
        public readonly int Seconds = 1200;
    }

    [MessageType(messageType: typeof(Per50MinuteEvent), description: "每50分钟事件", isCanNoHandler: true)]
    public class Per50MinuteEvent : EventBase {
        public readonly int Seconds = 3000;
    }

    [MessageType(messageType: typeof(Per100MinuteEvent), description: "每100分钟事件", isCanNoHandler: true)]
    public class Per100MinuteEvent : EventBase {
        public readonly int Seconds = 6000;
    }

    [MessageType(messageType: typeof(Per24HourEvent), description: "每24小时事件", isCanNoHandler: true)]
    public class Per24HourEvent : EventBase {
        public readonly int Seconds = 86400;
    }
}
