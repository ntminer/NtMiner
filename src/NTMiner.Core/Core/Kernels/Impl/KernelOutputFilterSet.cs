using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelOutputFilterSet : IKernelOutputFilterSet {
        private readonly Dictionary<Guid, KernelOutputFilterData> _dicById = new Dictionary<Guid, KernelOutputFilterData>();
        private readonly Dictionary<Guid, List<KernelOutputFilterData>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputFilterData>>();
        private readonly INTMinerRoot _root;
        private readonly bool _isUseJson;

        public KernelOutputFilterSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            _root.ServerContextWindow<AddKernelOutputFilterCommand>("添加内核输出过滤器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.RegexPattern)) {
                        throw new ValidationException($"{nameof(message.Input.RegexPattern)} can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputFilterData entity = new KernelOutputFilterData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    if (!_dicByKernelOutputId.ContainsKey(entity.KernelOutputId)) {
                        _dicByKernelOutputId.Add(entity.KernelOutputId, new List<KernelOutputFilterData>());
                    }
                    _dicByKernelOutputId[entity.KernelOutputId].Add(entity);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new KernelOutputFilterAddedEvent(entity));
                });
            _root.ServerContextWindow<UpdateKernelOutputFilterCommand>("更新内核输出过滤器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.RegexPattern)) {
                        throw new ValidationException($"{nameof(message.Input.RegexPattern)} can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputFilterData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new KernelOutputFilterUpdatedEvent(entity));
                });
            _root.ServerContextWindow<RemoveKernelOutputFilterCommand>("移除内核输出过滤器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelOutputFilterData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    _dicByKernelOutputId[entity.KernelOutputId].Remove(entity);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>(isUseJson);
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new KernelOutputFilterRemovedEvent(entity));
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>(_isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                        if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                            _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputFilterData>());
                        }
                        if (_dicByKernelOutputId[item.KernelOutputId].All(a => a.GetId() != item.GetId())) {
                            _dicByKernelOutputId[item.KernelOutputId].Add(item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid kernelOutputFilterId) {
            InitOnece();
            return _dicById.ContainsKey(kernelOutputFilterId);
        }

        public IEnumerable<IKernelOutputFilter> GetKernelOutputFilters(Guid kernelId) {
            InitOnece();
            if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                return _dicByKernelOutputId[kernelId];
            }
            return new List<IKernelOutputFilter>();
        }

        public bool TryGetKernelOutputFilter(Guid kernelOutputFilterId, out IKernelOutputFilter kernelOutputFilter) {
            InitOnece();
            KernelOutputFilterData f;
            var r = _dicById.TryGetValue(kernelOutputFilterId, out f);
            kernelOutputFilter = f;
            return r;
        }

        public IEnumerator<IKernelOutputFilter> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        private readonly Dictionary<IKernelOutputFilter, Regex> _regexDic = new Dictionary<IKernelOutputFilter, Regex>();
        private Regex GetRegex(IKernelOutputFilter kernelOutputFilter) {
            if (string.IsNullOrEmpty(kernelOutputFilter.RegexPattern)) {
                return null;
            }
            Regex regex;
            if (!_regexDic.ContainsKey(kernelOutputFilter)) {
                regex = new Regex(kernelOutputFilter.RegexPattern);
                _regexDic.Add(kernelOutputFilter, regex);
            }
            else {
                regex = _regexDic[kernelOutputFilter];
            }
            return regex;
        }

        public void Filter(Guid kernelId, ref string input) {
            try {
                InitOnece();
                if (string.IsNullOrEmpty(input)) {
                    return;
                }
                if (!_dicByKernelOutputId.ContainsKey(kernelId)) {
                    return;
                }
                foreach (var kernelOutputFilter in _dicByKernelOutputId[kernelId]) {
                    Regex regex = GetRegex(kernelOutputFilter);
                    if (regex == null) {
                        continue;
                    }
                    Match match = regex.Match(input);
                    if (match.Success) {
                        input = string.Empty;
                        break;
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
