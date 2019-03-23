using NTMiner.Bus;
using System;

namespace NTMiner.Language {
    [MessageType(messageType: typeof(RefreshLangSetCommand), description: "刷新语言")]
    public class RefreshLangSetCommand : Cmd {
        public RefreshLangSetCommand() {
        }
    }

    [MessageType(messageType: typeof(LangSetRefreshedEvent), description: "语言刷新后")]
    public class LangSetRefreshedEvent : EventBase {
        public LangSetRefreshedEvent() {
        }
    }

    [MessageType(messageType: typeof(RefreshLangViewItemSetCommand), description: "刷新语言项")]
    public class RefreshLangViewItemSetCommand : Cmd {
        public RefreshLangViewItemSetCommand() {
        }
    }

    [MessageType(messageType: typeof(LangViewItemSetRefreshedEvent), description: "语言项刷新后")]
    public class LangViewItemSetRefreshedEvent : EventBase {
        public LangViewItemSetRefreshedEvent() {
        }
    }

    [MessageType(messageType: typeof(GlobalLangChangedEvent), description: "切换语言后")]
    public class GlobalLangChangedEvent : DomainEvent<ILang> {
        public GlobalLangChangedEvent(ILang source) : base(source) {
        }
    }

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
    [MessageType(messageType: typeof(AddLangViewItemCommand), description: "添加语言项")]
    public class AddLangViewItemCommand : AddEntityCommand<ILangViewItem> {
        public AddLangViewItemCommand(ILangViewItem input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateLangViewItemCommand), description: "更新语言项")]
    public class UpdateLangViewItemCommand : UpdateEntityCommand<ILangViewItem> {
        public UpdateLangViewItemCommand(ILangViewItem input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveLangViewItemCommand), description: "移除语言项")]
    public class RemoveLangViewItemCommand : RemoveEntityCommand {
        public RemoveLangViewItemCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(LangViewItemAddedEvent), description: "添加了语言项后")]
    public class LangViewItemAddedEvent : DomainEvent<ILangViewItem> {
        public LangViewItemAddedEvent(ILangViewItem source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(LangViewItemUpdatedEvent), description: "更新了语言项后")]
    public class LangViewItemUpdatedEvent : DomainEvent<ILangViewItem> {
        public LangViewItemUpdatedEvent(ILangViewItem source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(LangViewItemRemovedEvent), description: "删除了语言项后")]
    public class LangViewItemRemovedEvent : DomainEvent<ILangViewItem> {
        public LangViewItemRemovedEvent(ILangViewItem source) : base(source) {
        }
    }
    #endregion
}
