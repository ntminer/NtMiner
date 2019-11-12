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
                Write.Stopwatch.Start();
#endif
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                this.Add = new DelegateCommand(() => {
                    if (!VirtualRoot.IsMinerStudio) {
                        return;
                    }
                    new UserViewModel().Edit.Execute(FormType.Add);
                });
                BuildEventPath<UserAddedEvent>("添加了用户后", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicByLoginName.ContainsKey(message.Source.LoginName)) {
                            _dicByLoginName.Add(message.Source.LoginName, new UserViewModel(message.Source));
                            OnPropertyChanged(nameof(List));
                        }
                    });
                BuildEventPath<UserUpdatedEvent>("更新了用户后", LogEnum.DevConsole,
                    action: message => {
                        UserViewModel vm;
                        if (_dicByLoginName.TryGetValue(message.Source.LoginName, out vm)) {
                            vm.Update(message.Source);
                        }
                    });
                BuildEventPath<UserRemovedEvent>("移除了用户后", LogEnum.DevConsole,
                    action: message => {
                        _dicByLoginName.Remove(message.Source.LoginName);
                        OnPropertyChanged(nameof(List));
                    });
                foreach (var item in NTMinerRoot.Instance.UserSet) {
                    _dicByLoginName.Add(item.LoginName, new UserViewModel(item));
                }
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
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
