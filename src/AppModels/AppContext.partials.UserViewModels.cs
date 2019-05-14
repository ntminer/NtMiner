using NTMiner.User;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class UserViewModels : ViewModelBase {
            public static readonly UserViewModels Instance = new UserViewModels();
            private readonly Dictionary<string, UserViewModel> _dicByLoginName = new Dictionary<string, UserViewModel>();

            public ICommand Add { get; private set; }

            private UserViewModels() {
#if DEBUG
                VirtualRoot.Stopwatch.Restart();
#endif
                if (Design.IsInDesignMode) {
                    return;
                }
                this.Add = new DelegateCommand(() => {
                    if (!VirtualRoot.IsMinerStudio) {
                        return;
                    }
                    new UserViewModel().Edit.Execute(FormType.Add);
                });
                On<UserAddedEvent>("添加了用户后", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicByLoginName.ContainsKey(message.Source.LoginName)) {
                            _dicByLoginName.Add(message.Source.LoginName, new UserViewModel(message.Source));
                            OnPropertyChanged(nameof(List));
                        }
                    });
                On<UserUpdatedEvent>("更新了用户后", LogEnum.DevConsole,
                    action: message => {
                        UserViewModel vm;
                        if (_dicByLoginName.TryGetValue(message.Source.LoginName, out vm)) {
                            vm.Update(message.Source);
                        }
                    });
                On<UserRemovedEvent>("移除了用户后", LogEnum.DevConsole,
                    action: message => {
                        _dicByLoginName.Remove(message.Source.LoginName);
                        OnPropertyChanged(nameof(List));
                    });
                foreach (var item in NTMinerRoot.Instance.UserSet) {
                    _dicByLoginName.Add(item.LoginName, new UserViewModel(item));
                }
#if DEBUG
                Write.DevWarn($"耗时{VirtualRoot.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            public List<UserViewModel> List {
                get {
                    return _dicByLoginName.Values.ToList();
                }
            }
        }
    }
}
