using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelSet : IKernelSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, KernelData> _dicById = new Dictionary<Guid, KernelData>();

        public KernelSet(INTMinerRoot root) {
            _root = root;
            Global.Access<RefreshKernelSetCommand>(
                Guid.Parse("3FE50ED8-1E60-4C3C-8486-DE478BBF9308"),
                "处理刷新内核数据集命令",
                LogEnum.Console,
                action: message => {
                    _isInited = false;
                    Init();
                });
            Global.Access<AddKernelCommand>(
                Guid.Parse("331be370-2d4f-488f-9dd8-3709e3ff63af"),
                "添加内核",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("package code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelData entity = new KernelData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Add(entity);

                    Global.Happened(new KernelAddedEvent(entity));
                });
            Global.Access<UpdateKernelCommand>(
                Guid.Parse("f23c801a-afbe-4e59-93c2-3eaecf3c7d8e"),
                "更新内核",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("package code can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Update(entity);

                    Global.Happened(new KernelUpdatedEvent(entity));
                });
            Global.Access<RemoveKernelCommand>(
                Guid.Parse("b90d68ba-2af2-48db-8bf3-5b2795667e8c"),
                "移除内核",
                LogEnum.Console,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelData entity = _dicById[message.EntityId];
                    List<Guid> coinKernelIds = root.CoinKernelSet.Where(a => a.KernelId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var coinKernelId in coinKernelIds) {
                        Global.Execute(new RemoveCoinKernelCommand(coinKernelId));
                    }
                    _dicById.Remove(entity.Id);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Remove(entity.Id);

                    Global.Happened(new KernelRemovedEvent(entity));
                });
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
        }

        private bool _isInited = false;
        private object _locker = new object();

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    _dicById.Clear();
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid packageId) {
            InitOnece();
            return _dicById.ContainsKey(packageId);
        }

        public bool TryGetKernel(Guid packageId, out IKernel package) {
            InitOnece();
            KernelData pkg;
            var r = _dicById.TryGetValue(packageId, out pkg);
            package = pkg;
            return r;
        }

        public IEnumerator<IKernel> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
