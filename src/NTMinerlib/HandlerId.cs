using System;
using System.Collections.Generic;

namespace NTMiner {
    public class HandlerId : IHandlerId {
        private static readonly Dictionary<string, HandlerId> _dicById = new Dictionary<string, HandlerId>();

        static HandlerId() {
            VirtualRoot.Accept<UpdateHandlerIdCommand>(
                "更新处理器日志配置",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Input.HandlerPath)) {
                        _dicById[message.Input.HandlerPath].LogType = message.Input.LogType;
                    }
                    else {
                        Create(typeof(UpdateHandlerIdCommand), typeof(HandlerId), message.Input.Description, message.Input.LogType);
                    }
                });
        }

        public static IHandlerId Create(Type messageType, Type location, string description, LogEnum logType) {
            string path = $"{location.FullName}[{messageType.FullName}]";
            if (_dicById.ContainsKey(path)) {
                var item = _dicById[path];
                item.MessageType = messageType;
                item.Location = location;
                item.Description = description;
                item.HandlerPath = path;
                VirtualRoot.Happened(new HandlerIdUpdatedEvent(item));
                return item;
            }
            else {
                var item = new HandlerId {
                    MessageType = messageType,
                    Location = location,
                    HandlerPath = path,
                    Description = description,
                    LogType = logType
                };
                _dicById.Add(path, item);
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

        public Type MessageType { get; set; }
        [LiteDB.BsonIgnore]
        public Type Location { get; set; }
        public string HandlerPath { get; set; }
        public LogEnum LogType { get; set; }
        public string Description { get; set; }
    }
}
