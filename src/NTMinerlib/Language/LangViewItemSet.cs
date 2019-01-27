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
            Global.Access<RefreshLangViewItemSetCommand>(
                Guid.Parse("90E8BC51-65FA-4192-BB94-F08BFADBC78E"),
                "处理刷新语言项命令",
                LogEnum.Console,
                action: message => {
                    _isInited = false;
                    Global.Happened(new LangViewItemSetRefreshedEvent());
                });
            Global.Access<AddLangViewItemCommand>(
                Guid.Parse("07AC4BE6-AB09-48D2-A3D7-8653EE52CC43"),
                "处理添加语言项命令",
                LogEnum.Console,
                action: message=> {
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    ILang lang;
                    if (LangSet.Instance.TryGetLang(message.Input.LangId, out lang)) {
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

                        Global.Happened(new LangViewItemAddedEvent(entity));
                    }
                });
            Global.Access<UpdateLangViewItemCommand>(
                Guid.Parse("CEC2EFC5-4F92-4226-ADCE-BE36D8968B9E"),
                "处理修改语言项命令",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        var entity = _dicById[message.Input.GetId()];
                        entity.Update(message.Input);
                        var repository = Repository.CreateLanguageRepository<LangViewItem>();
                        repository.Update(entity);

                        Global.Happened(new LangViewItemUpdatedEvent(entity));
                    }
                });
            Global.Access<RemoveLangViewItemCommand>(
                Guid.Parse("3827E59B-872D-45E6-8512-7EC22E1BE6E3"),
                "处理删除语言项命令",
                LogEnum.Console,
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

                        Global.Happened(new LangViewItemRemovedEvent(entity));
                    }
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
                    _dicByLangAndView.Clear();
                    _dicById.Clear();
                    IRepository<LangViewItem> repository = Repository.CreateLanguageRepository<LangViewItem>();
                    var langItems = repository.GetAll();
                    foreach (var lang in LangSet.Instance) {
                        var dic = new Dictionary<string, List<ILangViewItem>>();
                        _dicByLangAndView.Add(lang.GetId(), dic);
                    }
                    foreach (var kv in _dicByLangAndView) {
                        foreach (var item in langItems.Where(a => a.LangId == kv.Key)) {
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
            return dic[viewId].Cast<ILangViewItem>().ToList();
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
