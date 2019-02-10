using NTMiner.Bus;

namespace NTMiner.User {
    [MessageType(messageType: typeof(AddUserCommand), description: "添加用户")]
    public class AddUserCommand : Cmd {
        public AddUserCommand(IUser user) {
            this.User = user;
        }

        public IUser User {
            get; private set;
        }
    }

    [MessageType(messageType: typeof(ChangePasswordCommand), description: "更改密码")]
    public class ChangePasswordCommand : Cmd {
        public ChangePasswordCommand(string loginName, string oldPassword, string newPassword, string description) {
            this.LoginName = loginName;
            this.OldPassword = oldPassword;
            this.NewPassword = newPassword;
            this.Description = description;
        }

        public string LoginName { get; private set; }

        public string OldPassword { get; private set; }

        public string NewPassword { get; private set; }

        public string Description { get; private set; }
    }

    [MessageType(messageType: typeof(UpdateUserCommand), description: "更新用户")]
    public class UpdateUserCommand : Cmd {
        public UpdateUserCommand(IUser user) {
            this.User = user;
        }

        public IUser User {
            get; private set;
        }
    }

    [MessageType(messageType: typeof(RemoveUserCommand), description: "删除用户")]
    public class RemoveUserCommand : Cmd {
        public RemoveUserCommand(string loginName) {
            this.LoginName = loginName;
        }

        public string LoginName {
            get; private set;
        }
    }

    [MessageType(messageType: typeof(UserAddedEvent), description: "添加了新用户后")]
    public class UserAddedEvent : DomainEvent<IUser> {
        public UserAddedEvent(IUser source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(UserUpdatedEvent), description: "修改了用户后")]
    public class UserUpdatedEvent : DomainEvent<IUser> {
        public UserUpdatedEvent(IUser source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(UserRemovedEvent), description: "移除了用户后")]
    public class UserRemovedEvent : DomainEvent<IUser> {
        public UserRemovedEvent(IUser source) : base(source) {
        }
    }
}
