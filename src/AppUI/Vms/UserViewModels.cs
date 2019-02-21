using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class UserViewModels : ViewModelBase {
        public static readonly UserViewModels Current = new UserViewModels();

        private readonly Dictionary<string, UserViewModel> _dicByLoginName = new Dictionary<string, UserViewModel>();

        private UserViewModels() {
            VirtualRoot.On<UserAddedEvent>(
                "添加了用户后",
                LogEnum.Console,
                action: message => {

                });
            VirtualRoot.On<UserUpdatedEvent>(
                "更新了用户后",
                LogEnum.Console,
                action: message => {

                });
            VirtualRoot.On<UserAddedEvent>(
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
