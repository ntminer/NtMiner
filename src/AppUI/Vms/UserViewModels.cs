using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class UserViewModels : ViewModelBase {
        public static readonly UserViewModels Current = new UserViewModels();

        private readonly Dictionary<string, UserViewModel> _dicByLoginName = new Dictionary<string, UserViewModel>();

        private UserViewModels() {
            Global.Access<UserAddedEvent>(
                Guid.Parse("866DD6F9-2A66-4578-90C0-E55AA7791E2C"),
                "添加了用户后",
                LogEnum.Console,
                action: message => {

                });
            Global.Access<UserUpdatedEvent>(
                Guid.Parse("D03BD781-ABC3-415B-86E5-20AA8FF34862"),
                "更新了用户后",
                LogEnum.Console,
                action: message => {

                });
            Global.Access<UserAddedEvent>(
                Guid.Parse("7F039C84-A429-4110-97FB-9A740C0238EA"),
                "移除了用户后",
                LogEnum.Console,
                action: message => {

                });
        }

        public List<UserViewModel> List {
            get {
                return _dicByLoginName.Values.ToList();
            }
        }
    }
}
