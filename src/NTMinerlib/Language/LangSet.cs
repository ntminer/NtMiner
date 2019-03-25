using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language {
    public class LangSet : IEnumerable<ILang> {
        public static readonly LangSet Instance = new LangSet();

        private readonly List<Lang> _langs = new List<Lang>();

        private LangSet() {
            VirtualRoot.Window<RefreshLangSetCommand>("处理刷新语言命令", LogEnum.DevConsole,
                action: message => {
                    _isInited = false;
                    VirtualRoot.Happened(new LangSetRefreshedEvent());
                });

            VirtualRoot.Window<AddLangCommand>("处理添加语言命令", LogEnum.DevConsole,
                action: message => {
                    if (_langs.All(a => a.GetId() != message.Input.GetId() && a.Code != message.Input.Code)) {
                        Lang entity = new Lang().Update(message.Input);
                        _langs.Add(entity);
                        var repository = Repository.CreateLanguageRepository<Lang>();
                        repository.Add(entity);

                        VirtualRoot.Happened(new LangAddedEvent(entity));
                    }
                });
            VirtualRoot.Window<UpdateLangCommand>("处理修改语言命令", LogEnum.DevConsole,
                action: message => {
                    Lang entity = _langs.FirstOrDefault(a => a.GetId() == message.Input.GetId());
                    if (entity != null) {
                        entity.Update(message.Input);
                        var repository = Repository.CreateLanguageRepository<Lang>();
                        repository.Update(entity);

                        VirtualRoot.Happened(new LangUpdatedEvent(entity));
                    }
                });
            VirtualRoot.Window<RemoveLangCommand>("处理删除语言命令", LogEnum.DevConsole,
                action: message => {
                    var entity = _langs.FirstOrDefault(a => a.GetId() == message.EntityId);
                    if (entity != null) {
                        var toRemoveLangItemIds = new List<Guid>();
                        foreach (var g in LangViewItemSet.Instance.GetLangItems(message.EntityId)) {
                            foreach (var langItem in g.Value) {
                                toRemoveLangItemIds.Add(langItem.GetId());
                            }
                        }
                        foreach (var id in toRemoveLangItemIds) {
                            VirtualRoot.Execute(new RemoveLangViewItemCommand(id));
                        }
                        _langs.Remove(entity);
                        var repository = Repository.CreateLanguageRepository<Lang>();
                        repository.Remove(entity.GetId());

                        VirtualRoot.Happened(new LangRemovedEvent(entity));
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
                    IRepository<Lang> repository = Repository.CreateLanguageRepository<Lang>();
                    foreach (var item in repository.GetAll()) {
                        _langs.Add(item);
                    }
                    _isInited = true;
                }
            }
        }

        public ILang GetLangByCode(string langCode) {
            InitOnece();
            if (_langs == null || _langs.Count == 0) {
                return Lang.Empty;
            }
            if (_langs.Count == 1) {
                return _langs[0];
            }
            ILang result = _langs.FirstOrDefault(a => a.Code == langCode);
            if (result == null) {
                result = _langs.First();
            }
            return result;
        }

        public bool TryGetLang(string langCode, out ILang lang) {
            lang = _langs.FirstOrDefault(a => a.Code == langCode);
            return lang != null;
        }

        public bool TryGetLang(Guid langId, out ILang lang) {
            lang = _langs.FirstOrDefault(a => a.GetId() == langId);
            return lang != null;
        }

        public IEnumerator<ILang> GetEnumerator() {
            InitOnece();
            return _langs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _langs.GetEnumerator();
        }
    }
}
