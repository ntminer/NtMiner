using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Router {
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTypeAttribute : Attribute {
        private static readonly Dictionary<Type, MessageTypeAttribute> _messageTypeAttributeDic = new Dictionary<Type, MessageTypeAttribute>();
        public static MessageTypeAttribute GetMessageTypeAttribute(Type messageType) {
            if (_messageTypeAttributeDic.ContainsKey(messageType)) {
                return _messageTypeAttributeDic[messageType];
            }
            object atrrObj = messageType.GetCustomAttributes(typeof(MessageTypeAttribute), false).FirstOrDefault();
            MessageTypeAttribute attr;
            if (atrrObj == null) {
                attr = new MessageTypeAttribute(messageType.Name);
            }
            else {
                attr = (MessageTypeAttribute)atrrObj;
            }
            _messageTypeAttributeDic.Add(messageType, attr);

            return attr;
        }

        public MessageTypeAttribute(string description, bool isCanNoHandler = false) {
            this.Description = description;
            this.IsCanNoHandler = isCanNoHandler;
        }

        public string Description { get; private set; }
        /// <summary>
        /// 事件通常都会有响应，但有些事件不一定会有响应：比如<see cref="Per100MinuteEvent"/>、<see cref="HasBoot100MinuteEvent"/>等事件就不一定会有响应。
        /// 如果某些类型的事件确实不一定有响应时将该属性置为true表示“我知道该事件不一定有响应，不要再在DevConsole打印一行黄字警告我了！”
        /// </summary>
        public bool IsCanNoHandler { get; private set; }
    }
}
