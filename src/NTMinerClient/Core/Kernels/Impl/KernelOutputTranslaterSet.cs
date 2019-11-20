using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelOutputTranslaterSet : IKernelOutputTranslaterSet {
        public class SortNumberComparer : IComparer<ISortable> {
            public int Compare(ISortable x, ISortable y) {
                if (x == null || y == null) {
                    throw new ArgumentNullException();
                }
                return x.SortNumber - y.SortNumber;
            }
        }

        private readonly Dictionary<Guid, KernelOutputTranslaterData> _dicById = new Dictionary<Guid, KernelOutputTranslaterData>();
        private readonly Dictionary<Guid, List<KernelOutputTranslaterData>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputTranslaterData>>();
        public KernelOutputTranslaterSet(IServerContext context) {
            context.BuildCmdPath<AddKernelOutputTranslaterCommand>("添加内核输出翻译器", LogEnum.DevConsole,
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
                    KernelOutputTranslaterData entity = new KernelOutputTranslaterData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    if (!_dicByKernelOutputId.ContainsKey(entity.KernelOutputId)) {
                        _dicByKernelOutputId.Add(entity.KernelOutputId, new List<KernelOutputTranslaterData>());
                    }
                    _dicByKernelOutputId[entity.KernelOutputId].Add(entity);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new KernelOutputTranslaterAddedEvent(entity));
                });
            context.BuildCmdPath<UpdateKernelOutputTranslaterCommand>("更新内核输出翻译器", LogEnum.DevConsole,
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
                    KernelOutputTranslaterData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    string regexPattern = entity.RegexPattern;
                    entity.Update(message.Input);
                    _dicByKernelOutputId[entity.KernelOutputId].Sort(new SortNumberComparer());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new KernelOutputTranslaterUpdatedEvent(entity));
                });
            context.BuildCmdPath<RemoveKernelOutputTranslaterCommand>("移除内核输出翻译器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelOutputTranslaterData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    _dicByKernelOutputId[entity.KernelOutputId].Remove(entity);
                    _dicByKernelOutputId[entity.KernelOutputId].Sort(new SortNumberComparer());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new KernelOutputTranslaterRemovedEvent(entity));
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                        if (!_dicByKernelOutputId.ContainsKey(item.KernelOutputId)) {
                            _dicByKernelOutputId.Add(item.KernelOutputId, new List<KernelOutputTranslaterData>());
                        }
                        if (_dicByKernelOutputId[item.KernelOutputId].All(a => a.GetId() != item.GetId())) {
                            _dicByKernelOutputId[item.KernelOutputId].Add(item);
                        }
                    }
                    foreach (var item in _dicByKernelOutputId.Values) {
                        item.Sort(new SortNumberComparer());
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid consoleTranslaterId) {
            InitOnece();
            return _dicById.ContainsKey(consoleTranslaterId);
        }

        public IEnumerable<IKernelOutputTranslater> GetKernelOutputTranslaters(Guid kernelId) {
            InitOnece();
            if (_dicByKernelOutputId.ContainsKey(kernelId)) {
                return _dicByKernelOutputId[kernelId];
            }
            return new List<IKernelOutputTranslater>();
        }

        public bool TryGetKernelOutputTranslater(Guid consoleTranslaterId, out IKernelOutputTranslater consoleTranslater) {
            InitOnece();
            var r = _dicById.TryGetValue(consoleTranslaterId, out KernelOutputTranslaterData t);
            consoleTranslater = t;
            return r;
        }

        public IEnumerable<IKernelOutputTranslater> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }

        public void Translate(Guid kernelOutputId, ref string input, bool isPre = false) {
            try {
                InitOnece();
                if (string.IsNullOrEmpty(input) || !_dicByKernelOutputId.TryGetValue(kernelOutputId, out List<KernelOutputTranslaterData> translaters)) {
                    return;
                }
                foreach (var consoleTranslater in translaters) {
                    if (isPre && !consoleTranslater.IsPre) {
                        continue;
                    }
                    Regex regex = VirtualRoot.GetRegex(consoleTranslater.RegexPattern);
                    if (regex == null) {
                        continue;
                    }
                    Match match = regex.Match(input);
                    if (match.Success) {
                        string replacement = consoleTranslater.Replacement ?? string.Empty;
                        input = regex.Replace(input, replacement);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
