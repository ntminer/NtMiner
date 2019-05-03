using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Bus {
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTypeAttribute : Attribute {
        private static readonly Dictionary<Type, MessageTypeAttribute> SMessageTypeDescriptionDic = new Dictionary<Type, MessageTypeAttribute>();
        public static MessageTypeAttribute GetMessageTypeDescription(Type messageType) {
            if (SMessageTypeDescriptionDic.ContainsKey(messageType)) {
                return SMessageTypeDescriptionDic[messageType];
            }
            object atrrObj = messageType.GetCustomAttributes(typeof(MessageTypeAttribute), false).FirstOrDefault();
            MessageTypeAttribute messageTypeDescription;
            if (atrrObj == null) {
                messageTypeDescription = new MessageTypeAttribute(messageType.Name);
            }
            else {
                messageTypeDescription = (MessageTypeAttribute)atrrObj;
            }
            SMessageTypeDescriptionDic.Add(messageType, messageTypeDescription);

            return messageTypeDescription;
        }

        public MessageTypeAttribute(string description, bool isCanNoHandler = false) {
            this.Description = description;
            this.IsCanNoHandler = isCanNoHandler;
        }

        public string Description { get; private set; }
        /// <summary>
        /// 事件通常都会有响应，但有些事件不一定会有响应：比如<see cref="Per100MinuteEvent"/>、<see cref="HasBoot100MinuteEvent"/>等事件就不一定会有响应。
        /// 如果某些类型的事件确实不一定有响应时将该属性值为true表示“我知道该事件不一定有响应，不要再在DevConsole打印一行黄子警告我了！”
        /// </summary>
        public bool IsCanNoHandler { get; private set; }
    }
}
