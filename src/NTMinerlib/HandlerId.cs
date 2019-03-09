using System;
using System.Collections.Generic;

namespace NTMiner {
    public class HandlerId : IHandlerId {
        private static readonly Dictionary<string, HandlerId> SDicById = new Dictionary<string, HandlerId>();

        static HandlerId() {
            VirtualRoot.Accept<UpdateHandlerIdCommand>(
                "更新处理器日志配置",
                LogEnum.Console,
                action: message => {
                    if (SDicById.ContainsKey(message.Input.HandlerPath)) {
                        SDicById[message.Input.HandlerPath].LogType = message.Input.LogType;
                    }
                    else {
                        Create(typeof(UpdateHandlerIdCommand), typeof(HandlerId), message.Input.Description, message.Input.LogType);
                    }
                });
        }

        public static IHandlerId Create(Type messageType, Type location, string description, LogEnum logType) {
            string path = $"{location.FullName}[{messageType.FullName}]";
            if (SDicById.ContainsKey(path)) {
                var item = SDicById[path];
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
                SDicById.Add(path, item);
                if (VirtualRoot.IsPublishHandlerIdAddedEvent) {
                    VirtualRoot.Happened(new HandlerIdAddedEvent(item));
                }
                return item;
            }
        }

        public static IEnumerable<IHandlerId> GetHandlerIds() {
            return SDicById.Values;
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
