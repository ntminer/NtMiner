using NTMiner.Bus;
using System;

namespace NTMiner.Language {
    #region Lang Messages
    [MessageType(messageType: typeof(AddLangCommand), description: "添加语言")]
    public class AddLangCommand : AddEntityCommand<ILang> {
        public AddLangCommand(ILang input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateLangCommand), description: "更新语言")]
    public class UpdateLangCommand : UpdateEntityCommand<ILang> {
        public UpdateLangCommand(ILang input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveLangCommand), description: "删除语言")]
    public class RemoveLangCommand : RemoveEntityCommand {
        public RemoveLangCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(LangAddedEvent), description: "添加语言后")]
    public class LangAddedEvent : DomainEvent<ILang> {
        public LangAddedEvent(ILang source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(LangUpdatedEvent), description: "更新语言后")]
    public class LangUpdatedEvent : DomainEvent<ILang> {
        public LangUpdatedEvent(ILang source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(LangRemovedEvent), description: "删除语言后")]
    public class LangRemovedEvent : DomainEvent<ILang> {
        public LangRemovedEvent(ILang source) : base(source) {
        }
    }
    #endregion

    #region LangItem Messages
    [MessageType(messageType: typeof(AddLangItemCommand), description: "添加语言项")]
    public class AddLangItemCommand : AddEntityCommand<ILangItem> {
        public AddLangItemCommand(ILangItem input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateLangItemCommand), description: "更新语言项")]
    public class UpdateLangItemCommand : UpdateEntityCommand<ILangItem> {
        public UpdateLangItemCommand(ILangItem input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveLangItemCommand), description: "移除语言项")]
    public class RemoveLangItemCommand : RemoveEntityCommand {
        public RemoveLangItemCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(LangItemAddedEvent), description: "添加了语言项后")]
    public class LangItemAddedEvent : DomainEvent<ILangItem> {
        public LangItemAddedEvent(ILangItem source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(LangItemUpdatedEvent), description: "更新了语言项后")]
    public class LangItemUpdatedEvent : DomainEvent<ILangItem> {
        public LangItemUpdatedEvent(ILangItem source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(LangItemRemovedEvent), description: "删除了语言项后")]
    public class LangItemRemovedEvent : DomainEvent<ILangItem> {
        public LangItemRemovedEvent(ILangItem source) : base(source) {
        }
    }
    #endregion
}
