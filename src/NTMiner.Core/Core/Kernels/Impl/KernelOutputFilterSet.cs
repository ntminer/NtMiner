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

        public KernelOutputFilterSet(INTMinerRoot root) {
            _root = root;
            Global.Access<RefreshKernelOutputFilterSetCommand>(
                Guid.Parse("66DB9CB7-68C1-4654-8D49-4FA97F5B5188"),
                "处理刷新内核输出过滤器数据集命令",
                LogEnum.Console,
                action: message => {
                    _isInited = false;
                    Global.Happened(new KernelOutputFilterSetRefreshedEvent());
                });
            Global.Access<AddKernelOutputFilterCommand>(
                Guid.Parse("43c09cc6-456c-4e55-95b1-63b5937c5b11"),
                "添加内核输出过滤器",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.RegexPattern)) {
                        throw new ValidationException("KernelOutputFilter RegexPattern can't be null or empty");
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>();
                    repository.Add(entity);

                    Global.Happened(new KernelOutputFilterAddedEvent(entity));
                });
            Global.Access<UpdateKernelOutputFilterCommand>(
                Guid.Parse("b449bd25-98d8-4a60-9c75-36a6983c6176"),
                "更新内核输出过滤器",
                LogEnum.Log,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.RegexPattern)) {
                        throw new ValidationException("KernelOutputFilter RegexPattern can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputFilterData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>();
                    repository.Update(entity);

                    Global.Happened(new KernelOutputFilterUpdatedEvent(entity));
                });
            Global.Access<RemoveKernelOutputFilterCommand>(
                Guid.Parse("11a3a185-3d2e-463e-bd92-94a0db909d32"),
                "移除内核输出过滤器",
                LogEnum.Log,
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>();
                    repository.Remove(entity.Id);

                    Global.Happened(new KernelOutputFilterRemovedEvent(entity));
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
                    _dicById.Clear();
                    _dicByKernelOutputId.Clear();
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputFilterData>();
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
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
