using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class ColumnsShowViewModels : ViewModelBase {
            public static readonly ColumnsShowViewModels Instance = new ColumnsShowViewModels();

            private readonly Dictionary<Guid, ColumnsShowViewModel> _dicById = new Dictionary<Guid, ColumnsShowViewModel>();

            public ICommand Add { get; private set; }

            private ColumnsShowViewModels() {
#if DEBUG
                Write.Stopwatch.Start();
#endif
                this.Add = new DelegateCommand(() => {
                    new ColumnsShowViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AddEventPath<ColumnsShowAddedEvent>("添加了列显后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            ColumnsShowViewModel vm = new ColumnsShowViewModel(message.Target);
                            _dicById.Add(message.Target.GetId(), vm);
                            OnPropertyChanged(nameof(List));
                            AppContext.Instance.MinerClientsWindowVm.ColumnsShow = vm;
                        }
                    }, location: this.GetType());
                AddEventPath<ColumnsShowUpdatedEvent>("更新了列显后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.ContainsKey(message.Target.GetId())) {
                            ColumnsShowViewModel entity = _dicById[message.Target.GetId()];
                            entity.Update(message.Target);
                        }
                    }, location: this.GetType());
                AddEventPath<ColumnsShowRemovedEvent>("移除了列显后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        AppContext.Instance.MinerClientsWindowVm.ColumnsShow = _dicById.Values.FirstOrDefault();
                        _dicById.Remove(message.Target.GetId());
                        OnPropertyChanged(nameof(List));
                    }, location: this.GetType());
                foreach (var item in NTMinerRoot.Instance.ColumnsShowSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new ColumnsShowViewModel(item));
                }
#if DEBUG
                var elapsedMilliseconds = Write.Stopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            public List<ColumnsShowViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }
        }
    }
}
