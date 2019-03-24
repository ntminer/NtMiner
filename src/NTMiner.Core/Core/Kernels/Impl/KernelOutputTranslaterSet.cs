using NTMiner.Core.SysDics;
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
        private readonly bool _isUseJson;

        public KernelOutputTranslaterSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            VirtualRoot.Window<AddKernelOutputTranslaterCommand>("添加内核输出翻译器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.RegexPattern)) {
                        throw new ValidationException("ConsoleTranslater RegexPattern can't be null or empty");
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new KernelOutputTranslaterAddedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<UpdateKernelOutputTranslaterCommand>("更新内核输出翻译器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.RegexPattern)) {
                        throw new ValidationException("ConsoleTranslater RegexPattern can't be null or empty");
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
                    if (entity.RegexPattern != regexPattern) {
                        _regexDic.Remove(entity);
                    }
                    if (entity.Color != color) {
                        _colorDic.Remove(entity);
                    }
                    _dicByKernelOutputId[entity.KernelOutputId].Sort(new SortNumberComparer());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new KernelOutputTranslaterUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<RemoveKernelOutputTranslaterCommand>("移除内核输出翻译器", LogEnum.DevConsole,
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
                    _regexDic.Remove(entity);
                    _dicByKernelOutputId[entity.KernelOutputId].Sort(new SortNumberComparer());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>(isUseJson);
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new KernelOutputTranslaterRemovedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.On<SysDicItemUpdatedEvent>("LogColor字典项更新后刷新翻译器内存", LogEnum.DevConsole,
                action: message => {
                    ISysDic dic;
                    if (!_root.SysDicSet.TryGetSysDic("LogColor", out dic)) {
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
                }).AddToCollection(root.ContextHandlers);
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputTranslaterData>(_isUseJson);
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
            KernelOutputTranslaterData t;
            var r = _dicById.TryGetValue(consoleTranslaterId, out t);
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
                ISysDicItem dicItem;
                if (NTMinerRoot.Current.SysDicItemSet.TryGetDicItem("LogColor", consoleTranslater.Color, out dicItem)) {
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
            ConsoleColor consoleColor;
            if (color.TryParse(out consoleColor)) {
                return consoleColor;
            }
            return ConsoleColor.White;
        }

        private static int GetRgbValue(char letter) {
            int n = letter;
            if (n >= 87) {
                n = n - 87;
            }
            else if (n >= 55) {
                n = n - 55;
            }
            else if (n >= 48) {
                n = n - 48;
            }
            return n;
        }

        private readonly Dictionary<IKernelOutputTranslater, Regex> _regexDic = new Dictionary<IKernelOutputTranslater, Regex>();
        private Regex GetRegex(IKernelOutputTranslater consoleTranslater) {
            if (string.IsNullOrEmpty(consoleTranslater.RegexPattern)) {
                return null;
            }
            Regex regex;
            if (!_regexDic.ContainsKey(consoleTranslater)) {
                regex = new Regex(consoleTranslater.RegexPattern);
                _regexDic.Add(consoleTranslater, regex);
            }
            else {
                regex = _regexDic[consoleTranslater];
            }
            return regex;
        }

        public void Translate(Guid kernelId, ref string input, ref ConsoleColor color, bool isPre = false) {
            try {
                InitOnece();
                if (string.IsNullOrEmpty(input)) {
                    return;
                }
                if (!_dicByKernelOutputId.ContainsKey(kernelId)) {
                    return;
                }
                foreach (var consoleTranslater in _dicByKernelOutputId[kernelId]) {
                    if (isPre && !consoleTranslater.IsPre) {
                        continue;
                    }
                    Regex regex = GetRegex(consoleTranslater);
                    if (regex == null) {
                        continue;
                    }
                    Match match = regex.Match(input);
                    if (match.Success) {
                        if (VirtualRoot.Lang.Code == "zh-CN" || (isPre && consoleTranslater.IsPre)) {
                            if (!string.IsNullOrEmpty(consoleTranslater.Replacement)) {
                                input = regex.Replace(input, consoleTranslater.Replacement);
                            }
                        }
                        if (!string.IsNullOrEmpty(consoleTranslater.Color)) {
                            color = GetColor(consoleTranslater);
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
