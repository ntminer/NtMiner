using NTMiner.User;
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
                    if (!_dicByLoginName.ContainsKey(message.Source.LoginName)) {
                        _dicByLoginName.Add(message.Source.LoginName, new UserViewModel(message.Source));
                        OnPropertyChanged(nameof(List));
                    }
                });
            VirtualRoot.On<UserUpdatedEvent>(
                "更新了用户后",
                LogEnum.Console,
                action: message => {
                    UserViewModel vm;
                    if (_dicByLoginName.TryGetValue(message.Source.LoginName, out vm)) {
                        vm.Update(message.Source);
                    }
                });
            VirtualRoot.On<UserAddedEvent>(
                "移除了用户后",
                LogEnum.Console,
                action: message => {
                    _dicByLoginName.Remove(message.Source.LoginName);
                    OnPropertyChanged(nameof(List));
                });
        }

        public List<UserViewModel> List {
            get {
                return _dicByLoginName.Values.ToList();
            }
        }
    }
}
