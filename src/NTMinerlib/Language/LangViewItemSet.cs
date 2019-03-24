using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language {
    public class LangViewItemSet : IEnumerable<LangViewItem> {
        public static readonly LangViewItemSet Instance = new LangViewItemSet();

        private readonly Dictionary<Guid, Dictionary<string, List<ILangViewItem>>> _dicByLangAndView = new Dictionary<Guid, Dictionary<string, List<ILangViewItem>>>();
        private readonly Dictionary<Guid, LangViewItem> _dicById = new Dictionary<Guid, LangViewItem>();

        private LangViewItemSet() {
            VirtualRoot.Window<RefreshLangViewItemSetCommand>("处理刷新语言项命令", LogEnum.DevConsole,
                action: message => {
                    _isInited = false;
                    VirtualRoot.Happened(new LangViewItemSetRefreshedEvent());
                });
            VirtualRoot.Window<AddLangViewItemCommand>("处理添加语言项命令", LogEnum.DevConsole,
                action: message=> {
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }

                    if (LangSet.Instance.TryGetLang(message.Input.LangId, out _)) {
                        if (!_dicByLangAndView.ContainsKey(message.Input.LangId)) {
                            _dicByLangAndView.Add(message.Input.LangId, new Dictionary<string, List<ILangViewItem>>());
                        }
                        var dic = _dicByLangAndView[message.Input.LangId];
                        if (!dic.ContainsKey(message.Input.ViewId)) {
                            dic.Add(message.Input.ViewId, new List<ILangViewItem>());
                        }
                        var entity = new LangViewItem().Update(message.Input);
                        dic[message.Input.ViewId].Add(entity);
                        _dicById.Add(message.Input.GetId(), entity);
                        var repository = Repository.CreateLanguageRepository<LangViewItem>();
                        repository.Add(entity);

                        VirtualRoot.Happened(new LangViewItemAddedEvent(entity));
                    }
                });
            VirtualRoot.Window<UpdateLangViewItemCommand>("处理修改语言项命令", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        var entity = _dicById[message.Input.GetId()];
                        entity.Update(message.Input);
                        var repository = Repository.CreateLanguageRepository<LangViewItem>();
                        repository.Update(entity);

                        VirtualRoot.Happened(new LangViewItemUpdatedEvent(entity));
                    }
                });
            VirtualRoot.Window<RemoveLangViewItemCommand>("处理删除语言项命令", LogEnum.DevConsole,
                action: message => {
                    if (_dicById.ContainsKey(message.EntityId)) {
                        var entity = _dicById[message.EntityId];
                        _dicById.Remove(message.EntityId);
                        ILang lang;
                        if (LangSet.Instance.TryGetLang(entity.LangId, out lang)) {
                            if (_dicByLangAndView.ContainsKey(entity.LangId)) {
                                var dic = _dicByLangAndView[entity.LangId];
                                if (dic.ContainsKey(entity.ViewId) && dic[entity.ViewId].Contains(entity)) {
                                    dic[entity.ViewId].Remove(entity);
                                    if (dic[entity.ViewId].Count == 0) {
                                        dic.Remove(entity.ViewId);
                                    }
                                }
                                if (_dicByLangAndView.Count == 0) {
                                    _dicByLangAndView.Remove(entity.LangId);
                                }
                            }
                        }
                        var repository = Repository.CreateLanguageRepository<LangViewItem>();
                        repository.Remove(entity.Id);

                        VirtualRoot.Happened(new LangViewItemRemovedEvent(entity));
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
                    _dicByLangAndView.Clear();
                    _dicById.Clear();
                    IRepository<LangViewItem> repository = Repository.CreateLanguageRepository<LangViewItem>();
                    var langItems = repository.GetAll();
                    foreach (var lang in LangSet.Instance) {
                        var dic = new Dictionary<string, List<ILangViewItem>>();
                        _dicByLangAndView.Add(lang.GetId(), dic);
                    }
                    var langViewItems = langItems as LangViewItem[] ?? langItems.ToArray();
                    foreach (var kv in _dicByLangAndView) {
                        foreach (var item in langViewItems.Where(a => a.LangId == kv.Key)) {
                            if (!kv.Value.ContainsKey(item.ViewId)) {
                                kv.Value.Add(item.ViewId, new List<ILangViewItem>());
                            }
                            kv.Value[item.ViewId].Add(item);
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public List<ILangViewItem> GetLangItems(Guid langId, string viewId) {
            InitOnece();
            if (!_dicByLangAndView.ContainsKey(langId)) {
                return new List<ILangViewItem>();
            }
            var dic = _dicByLangAndView[langId];
            if (!dic.ContainsKey(viewId)) {
                return new List<ILangViewItem>();
            }
            return dic[viewId].ToList();
        }

        public Dictionary<string, List<ILangViewItem>> GetLangItems(Guid langId) {
            InitOnece();
            if (!_dicByLangAndView.ContainsKey(langId)) {
                return new Dictionary<string, List<ILangViewItem>>();
            }
            return _dicByLangAndView[langId];
        }

        public IEnumerator<LangViewItem> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
