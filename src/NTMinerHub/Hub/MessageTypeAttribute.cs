using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Hub {
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTypeAttribute : Attribute {
        private static readonly Dictionary<Type, MessageTypeAttribute> _messageTypeAttributeDic = new Dictionary<Type, MessageTypeAttribute>();
        private static readonly object _locker = new object();
        public static MessageTypeAttribute GetMessageTypeAttribute(Type messageType) {
            if (_messageTypeAttributeDic.TryGetValue(messageType, out MessageTypeAttribute attr)) {
                return attr;
            }
            lock (_locker) {
                if (_messageTypeAttributeDic.TryGetValue(messageType, out attr)) {
                    return attr;
                }
                object atrrObj = messageType.GetCustomAttributes(typeof(MessageTypeAttribute), false).FirstOrDefault();
                if (atrrObj == null) {
                    attr = new MessageTypeAttribute(messageType.Name);
                }
                else {
                    attr = (MessageTypeAttribute)atrrObj;
                }
                _messageTypeAttributeDic.Add(messageType, attr);

                return attr;
            }
        }

        public MessageTypeAttribute(string description, bool isCanNoPath = false) {
            this.Description = description;
            this.IsCanNoPath = isCanNoPath;
        }

        public string Description { get; private set; }
        /// <summary>
        /// 事件通常都会有传播路径，但有些事件不一定会有传播路径：比如<see cref="Per100MinuteEvent"/>、<see cref="HasBoot100MinuteEvent"/>等事件就不一定会被订阅从而没有传播路径。
        /// 如果某些类型的事件确实不一定有传播路径时将该属性置为true表示“我知道该事件不一定有传播路径，不要再在DevConsole打印一行黄字警告我了！”
        /// </summary>
        public bool IsCanNoPath { get; private set; }
    }
}
