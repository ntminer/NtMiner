using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelOutputSet : IKernelOutputSet {
        private readonly Dictionary<Guid, KernelOutputData> _dicById = new Dictionary<Guid, KernelOutputData>();

        private readonly INTMinerRoot _root;

        public KernelOutputSet(INTMinerRoot root) {
            _root = root;
            Global.Access<AddKernelOutputCommand>(
                Guid.Parse("142AE86A-C264-40B2-A617-D65E33C7FEE2"),
                "添加内核输出组",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputData entity = new KernelOutputData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    repository.Add(entity);

                    Global.Happened(new KernelOutputAddedEvent(entity));
                });
            Global.Access<UpdateKernelOutputCommand>(
                Guid.Parse("2A3CAE7E-D0E2-4E4B-B75B-357EB0BE1AA1"),
                "更新内核输出组",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("KernelOutput name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    repository.Update(entity);

                    Global.Happened(new KernelOutputUpdatedEvent(entity));
                });
            Global.Access<RemoveKernelOutputCommand>(
                Guid.Parse("43B565B4-1509-4DC6-9FA4-55D49C79C60A"),
                "移除内核输出组",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelOutputData entity = _dicById[message.EntityId];
                    List<Guid> kernelOutputFilterIds = root.KernelOutputFilterSet.Where(a => a.KernelOutputId == entity.Id).Select(a => a.GetId()).ToList();
                    List<Guid> kernelOutputTranslaterIds = root.KernelOutputTranslaterSet.Where(a => a.KernelOutputId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var kernelOutputFilterId in kernelOutputFilterIds) {
                        Global.Execute(new RemoveKernelOutputFilterCommand(kernelOutputFilterId));
                    }
                    foreach (var kernelOutputTranslaterId in kernelOutputTranslaterIds) {
                        Global.Execute(new RemoveKernelOutputTranslaterCommand(kernelOutputTranslaterId));
                    }
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    repository.Remove(message.EntityId);

                    Global.Happened(new KernelOutputRemovedEvent(entity));
                });
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
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

        public bool TryGetKernelOutput(Guid id, out IKernelOutput kernelOutput) {
            InitOnece();
            KernelOutputData data;
            var result = _dicById.TryGetValue(id, out data);
            kernelOutput = data;
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        public IEnumerator<IKernelOutput> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
