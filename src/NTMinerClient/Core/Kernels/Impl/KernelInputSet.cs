using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelInputSet : IKernelInputSet {
        private readonly Dictionary<Guid, KernelInputData> _dicById = new Dictionary<Guid, KernelInputData>();

        public KernelInputSet(IServerContext context) {
            context.BuildCmdPath<AddKernelInputCommand>("添加内核输入组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelInputData entity = new KernelInputData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new KernelInputAddedEvent(message.Id, entity));
                });
            context.BuildCmdPath<UpdateKernelInputCommand>("更新内核输入组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException($"{nameof(message.Input.Name)} can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelInputData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new KernelInputUpdatedEvent(message.Id, entity));
                });
            context.BuildCmdPath<RemoveKernelInputCommand>("移除内核输入组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelInputData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new KernelInputRemovedEvent(message.Id, entity));
                });
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public bool TryGetKernelInput(Guid id, out IKernelInput kernelInput) {
            InitOnece();
            var result = _dicById.TryGetValue(id, out KernelInputData data);
            kernelInput = data;
            return result;
        }

        public IEnumerable<IKernelInput> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
