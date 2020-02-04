using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class FragmentWriterSet : IFragmentWriterSet {
        private readonly Dictionary<Guid, FragmentWriterData> _dicById = new Dictionary<Guid, FragmentWriterData>();

        public FragmentWriterSet(IServerContext context) {
            context.AddCmdPath<AddFragmentWriterCommand>("添加命令行片段书写器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (string.IsNullOrEmpty(message.Input.Body)) {
                        throw new ValidationException("FragmentWriter body can't be null or empty");
                    }
                    FragmentWriterData entity = new FragmentWriterData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new FragmentWriterAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdateFragmentWriterCommand>("更新命令行片段书写器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Body)) {
                        throw new ValidationException("FragmentWriter body can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    FragmentWriterData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new FragmentWriterUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveFragmentWriterCommand>("移除组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    FragmentWriterData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new FragmentWriterRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetFragmentWriter(Guid writerId, out IFragmentWriter writer) {
            InitOnece();
            bool r = _dicById.TryGetValue(writerId, out FragmentWriterData g);
            writer = g;
            return r;
        }

        public IEnumerable<IFragmentWriter> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
