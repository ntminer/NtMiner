using NTMiner.Bus;
using System;

namespace NTMiner.FileETag {
    #region ETag Messages
    [MessageType(messageType: typeof(AddETagCommand), description: "添加ETag")]
    public class AddETagCommand : AddEntityCommand<IETag> {
        public AddETagCommand(IETag input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateETagCommand), description: "更新ETag")]
    public class UpdateETagCommand : UpdateEntityCommand<IETag> {
        public UpdateETagCommand(IETag input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveETagCommand), description: "删除ETag")]
    public class RemoveETagCommand : RemoveEntityCommand {
        public RemoveETagCommand(Guid entityId, string key) : base(entityId) {
            this.Key = key;
        }

        public string Key { get; private set; }
    }

    [MessageType(messageType: typeof(ETagAddedEvent), description: "添加ETag后")]
    public class ETagAddedEvent : DomainEvent<IETag> {
        public ETagAddedEvent(IETag source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(ETagUpdatedEvent), description: "更新ETag后")]
    public class ETagUpdatedEvent : DomainEvent<IETag> {
        public ETagUpdatedEvent(IETag source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(ETagRemovedEvent), description: "删除ETag后")]
    public class ETagRemovedEvent : DomainEvent<IETag> {
        public ETagRemovedEvent(IETag source) : base(source) {
        }
    }
    #endregion
}
