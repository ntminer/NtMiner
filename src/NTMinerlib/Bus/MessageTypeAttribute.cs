using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Bus {
    public class MessageTypeAttribute : Attribute {
        private static readonly Dictionary<Type, MessageTypeAttribute> s_messageTypeDescriptionDic = new Dictionary<Type, MessageTypeAttribute>();
        public static MessageTypeAttribute GetMessageTypeDescription(Type messageType) {
            if (s_messageTypeDescriptionDic.ContainsKey(messageType)) {
                return s_messageTypeDescriptionDic[messageType];
            }
            object atrrObj = messageType.GetCustomAttributes(typeof(MessageTypeAttribute), false).FirstOrDefault();
            MessageTypeAttribute messageTypeDescription;
            if (atrrObj == null) {
                messageTypeDescription = new MessageTypeAttribute(messageType, messageType.Name);
            }
            else {
                messageTypeDescription = (MessageTypeAttribute)atrrObj;
            }
            s_messageTypeDescriptionDic.Add(messageType, messageTypeDescription);

            return messageTypeDescription;
        }

        public MessageTypeAttribute(Type messageType, string description, bool isCanNoHandler = false) {
            this.MessageType = messageType;
            this.Description = description;
            this.IsCanNoHandler = isCanNoHandler;
        }

        public Type MessageType { get; private set; }
        public string Description { get; private set; }
        public bool IsCanNoHandler { get; private set; }
    }
}
