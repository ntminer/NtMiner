using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class FragmentWriterSet : IFragmentWriterSet {
        private readonly Dictionary<Guid, FragmentWriterData> _dicById = new Dictionary<Guid, FragmentWriterData>();

        private readonly INTMinerRoot _root;
        private readonly bool _isUseJson;

        public FragmentWriterSet(INTMinerRoot root, bool isUseJson) {
            _isUseJson = isUseJson;
            _root = root;
            _root.ServerContextWindow<AddFragmentWriterCommand>("添加命令行片段书写器", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new FragmentWriterAddedEvent(entity));
                });
            _root.ServerContextWindow<UpdateFragmentWriterCommand>("更新命令行片段书写器", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new FragmentWriterUpdatedEvent(entity));
                });
            _root.ServerContextWindow<RemoveFragmentWriterCommand>("移除组", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>(isUseJson);
                    repository.Remove(message.EntityId);

                    VirtualRoot.Happened(new FragmentWriterRemovedEvent(entity));
                });
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<FragmentWriterData>(_isUseJson);
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
            FragmentWriterData g;
            bool r = _dicById.TryGetValue(writerId, out g);
            writer = g;
            return r;
        }

        public IEnumerator<IFragmentWriter> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
