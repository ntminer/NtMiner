using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelInputSet : IKernelInputSet {
        private readonly Dictionary<Guid, KernelInputData> _dicById = new Dictionary<Guid, KernelInputData>();

        private readonly INTMinerRoot _root;
        private readonly bool _isUseJson;

        public KernelInputSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            VirtualRoot.Accept<RefreshKernelInputSetCommand>(
                "处理刷新内核输入数据集命令",
                LogEnum.Console,
                action: message => {
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>(isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (_dicById.ContainsKey(item.Id)) {
                            VirtualRoot.Execute(new UpdateKernelInputCommand(item));
                        }
                        else {
                            VirtualRoot.Execute(new AddKernelInputCommand(item));
                        }
                    }
                });
            VirtualRoot.Accept<AddKernelInputCommand>(
                "添加内核输入组",
                LogEnum.Console,
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new KernelInputAddedEvent(entity));
                });
            VirtualRoot.Accept<UpdateKernelInputCommand>(
                "更新内核输入组",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("KernelInput name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelInputData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new KernelInputUpdatedEvent(entity));
                });
            VirtualRoot.Accept<RemoveKernelInputCommand>(
                "移除内核输入组",
                LogEnum.Console,
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>(isUseJson);
                    repository.Remove(message.EntityId);

                    VirtualRoot.Happened(new KernelInputRemovedEvent(entity));
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelInputData>(_isUseJson);
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
            KernelInputData data;
            var result = _dicById.TryGetValue(id, out data);
            kernelInput = data;
            return result;
        }

        public IEnumerator<IKernelInput> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
