using System;
using System.Collections.Generic;

namespace NTMiner {
    public class HandlerId : IHandlerId, IDbEntity<Guid> {
        private static readonly Dictionary<Guid, HandlerId> _dicById = new Dictionary<Guid, HandlerId>();

        static HandlerId() {
            VirtualRoot.Access<UpdateHandlerIdCommand>(
                Guid.Parse("77acf9fd-5e2f-464e-be81-5095d830962b"),
                "更新处理器日志配置",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Input.Id)) {
                        _dicById[message.Input.Id].LogType = message.Input.LogType;
                    }
                    else {
                        Create(typeof(UpdateHandlerIdCommand), typeof(HandlerId), message.Input.Id, message.Input.Description, message.Input.LogType);
                    }
                });
        }

        public static IHandlerId Create(Type messageType, Type location, Guid id, string description, LogEnum logType) {
            if (_dicById.ContainsKey(id)) {
                var item = _dicById[id];
                item.MessageType = messageType;
                item.Location = location;
                item.Description = description;
                VirtualRoot.Happened(new HandlerIdUpdatedEvent(item));
                return item;
            }
            else {
                var item = new HandlerId {
                    MessageType = messageType,
                    Location = location,
                    Id = id,
                    Description = description,
                    LogType = logType
                };
                _dicById.Add(id, item);
                if (VirtualRoot.IsPublishHandlerIdAddedEvent) {
                    VirtualRoot.Happened(new HandlerIdAddedEvent(item));
                }
                return item;
            }
        }

        public static IEnumerable<IHandlerId> GetHandlerIds() {
            return _dicById.Values;
        }

        private HandlerId() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public Type MessageType { get; set; }
        [LiteDB.BsonIgnore]
        public Type Location { get; set; }
        public LogEnum LogType { get; set; }
        public string Description { get; set; }
    }
}
