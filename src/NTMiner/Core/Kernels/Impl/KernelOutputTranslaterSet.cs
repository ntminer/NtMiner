using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelOutputTranslaterSet : IKernelOutputTranslaterSet {
        private readonly Dictionary<Guid, KernelOutputTranslaterData> _dicById = new Dictionary<Guid, KernelOutputTranslaterData>();
        private readonly Dictionary<Guid, List<KernelOutputTranslaterData>> _dicByKernelOutputId = new Dictionary<Guid, List<KernelOutputTranslaterData>>();
        private readonly INTMinerRoot _root;
        public KernelOutputTranslaterSet(INTMinerRoot root) {
            _root = root;
            _root.ServerContextCmdPath<AddKernelOutputTranslaterCommand>("添加内核输出翻译器", LogEnum.DevConsole,
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

                    VirtualRoot.Happened(new KernelOutputTranslaterAddedEvent(entity));
                });
            _root.ServerContextCmdPath<UpdateKernelOutputTranslaterCommand>("更新内核输出翻译器", LogEnum.DevConsole,
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
                    string color = entity.Color;
                    entity.Update(message.Input);
                    if (entity.Color != color) {
                        _colorDic.Remove(entity);
                    }
                    _dicByKernelOutputId[entity.KernelOutputId].Sort(new SortNumberComparer());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>();
                    repository.Update(entity);

                    VirtualRoot.Happened(new KernelOutputTranslaterUpdatedEvent(entity));
                });
            _root.ServerContextCmdPath<RemoveKernelOutputTranslaterCommand>("移除内核输出翻译器", LogEnum.DevConsole,
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
                    _colorDic.Remove(entity);
                    _dicByKernelOutputId[entity.KernelOutputId].Sort(new SortNumberComparer());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new KernelOutputTranslaterRemovedEvent(entity));
                });
            _root.ServerContextEventPath<SysDicItemUpdatedEvent>($"{Consts.LogColorSysDicCode}字典项更新后刷新翻译器内存", LogEnum.DevConsole,
                action: message => {
                    if (!_root.SysDicSet.TryGetSysDic(Consts.LogColorSysDicCode, out ISysDic dic)) {
                        return;
                    }
                    if (message.Source.DicId != dic.GetId()) {
                        return;
                    }
                    foreach (var entity in _dicById.Values) {
                        if (entity.Color == message.Source.Code) {
                            _colorDic.Remove(entity);
                        }
                    }
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

        public IEnumerator<IKernelOutputTranslater> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        private Dictionary<IKernelOutputTranslater, ConsoleColor> _colorDic = new Dictionary<IKernelOutputTranslater, ConsoleColor>();
        private ConsoleColor GetColor(IKernelOutputTranslater consoleTranslater) {
            if (!_colorDic.ContainsKey(consoleTranslater)) {
                if (NTMinerRoot.Instance.SysDicItemSet.TryGetDicItem(Consts.LogColorSysDicCode, consoleTranslater.Color, out ISysDicItem dicItem)) {
                    _colorDic.Add(consoleTranslater, GetColor(dicItem.Value));
                }
                else {
                    _colorDic.Add(consoleTranslater, GetColor(consoleTranslater.Color));
                }
            }
            return _colorDic[consoleTranslater];
        }

        public static ConsoleColor GetColor(string color) {
            if (string.IsNullOrEmpty(color)) {
                return ConsoleColor.White;
            }
            color = color.Trim();
            if (string.IsNullOrEmpty(color)) {
                return ConsoleColor.White;
            }
            if (color.TryParse(out ConsoleColor consoleColor)) {
                return consoleColor;
            }
            return ConsoleColor.White;
        }

        public void Translate(Guid kernelOutputId, ref string input, ref ConsoleColor color, bool isPre = false) {
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
                        if (!string.IsNullOrEmpty(consoleTranslater.Color)) {
                            color = GetColor(consoleTranslater);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }
    }
}
