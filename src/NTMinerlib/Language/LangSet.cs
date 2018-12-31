using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language {
    public class LangSet : IEnumerable<ILang> {
        public static readonly LangSet Instance = new LangSet();

        private readonly List<Lang> _langs = new List<Lang>() {
            new Lang {
                Id = new Guid("584398F7-5BDD-41B3-94C6-50A14F23DB71"),
                Code = "en",
                Name = "english"
            },
            new Lang {
                Id = new Guid("9DD5F05A-003E-4A9D-91CA-38548D960BD4"),
                Code = "cn-zh",
                Name = "中文简体"
            }
        };

        private LangSet() {
            Global.Access<AddLangCommand>(
                Guid.Parse("75818F82-D124-48B0-8138-D150D77EC557"),
                "处理添加语言命令",
                LogEnum.None,
                action: message => {
                    if (_langs.All(a => a.GetId() != message.Input.GetId() && a.Code != message.Input.Code)) {
                        Lang entity = new Lang().Update(message.Input);
                        _langs.Add(entity);
                        var repository = Repository.CreateLanguageRepository<Lang>();
                        repository.Add(entity);
                    }
                });
            Global.Access<UpdateLangCommand>(
                Guid.Parse("C6D2436E-C255-433F-8FAF-4E1D00570BF1"),
                "处理修改语言命令",
                LogEnum.None,
                action: message => {
                    Lang entity = _langs.FirstOrDefault(a => a.GetId() == message.Input.GetId());
                    if (entity != null) {
                        entity.Update(message.Input);
                        var repository = Repository.CreateLanguageRepository<Lang>();
                        repository.Update(entity);
                    }
                });
            Global.Access<RemoveLangCommand>(
                Guid.Parse("8C421769-23EB-4FF0-A634-A5C2DC58CD92"),
                "处理删除语言命令",
                LogEnum.None,
                action: message => {
                    var entity = _langs.FirstOrDefault(a => a.GetId() == message.EntityId);
                    if (entity != null) {
                        var toRemoveLangItemIds = new List<Guid>();
                        foreach (var g in LangItemSet.Instance.GetLangItems(entity)) {
                            foreach (var langItem in g.Value) {
                                toRemoveLangItemIds.Add(langItem.GetId());
                            }
                        }
                        foreach (var id in toRemoveLangItemIds) {
                            Global.Execute(new RemoveLangItemCommand(id));
                        }
                        _langs.Remove(entity);
                        var repository = Repository.CreateLanguageRepository<Lang>();
                        repository.Remove(entity.GetId());
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
                throw new GeneralException("语言包为空");
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
