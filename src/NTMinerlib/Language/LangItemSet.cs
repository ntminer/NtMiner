using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language {
    public class LangItemSet {
        public static readonly LangItemSet Instance = new LangItemSet();

        private readonly Dictionary<ILang, Dictionary<string, List<ILangItem>>> _dicByLangAndView = new Dictionary<ILang, Dictionary<string, List<ILangItem>>>();
        private readonly Dictionary<Guid, LangItem> _dicById = new Dictionary<Guid, LangItem>();

        private LangItemSet() {
            Global.Access<AddLangItemCommand>(
                Guid.Parse("07AC4BE6-AB09-48D2-A3D7-8653EE52CC43"),
                "处理添加语言项命令",
                LogEnum.None,
                action: message=> {
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    ILang lang;
                    if (LangSet.Instance.TryGetLang(message.Input.LangId, out lang)) {
                        if (!_dicByLangAndView.ContainsKey(lang)) {
                            _dicByLangAndView.Add(lang, new Dictionary<string, List<ILangItem>>());
                        }
                        var dic = _dicByLangAndView[lang];
                        if (!dic.ContainsKey(message.Input.ViewId)) {
                            dic.Add(message.Input.ViewId, new List<ILangItem>());
                        }
                        var entity = new LangItem().Update(message.Input);
                        dic[message.Input.ViewId].Add(entity);
                        _dicById.Add(message.Input.GetId(), entity);
                        var repository = Repository.CreateLanguageRepository<LangItem>();
                        repository.Add(entity);
                    }
                });
            Global.Access<UpdateLangItemCommand>(
                Guid.Parse("CEC2EFC5-4F92-4226-ADCE-BE36D8968B9E"),
                "处理修改语言项命令",
                LogEnum.None,
                action: message => {
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        var entity = _dicById[message.Input.GetId()];
                        entity.Update(message.Input);
                        var repository = Repository.CreateLanguageRepository<LangItem>();
                        repository.Update(entity);
                    }
                });
            Global.Access<RemoveLangItemCommand>(
                Guid.Parse("3827E59B-872D-45E6-8512-7EC22E1BE6E3"),
                "处理删除语言项命令",
                LogEnum.None,
                action: message => {
                    if (_dicById.ContainsKey(message.EntityId)) {
                        var entity = _dicById[message.EntityId];
                        _dicById.Remove(message.EntityId);
                        ILang lang;
                        if (LangSet.Instance.TryGetLang(entity.LangId, out lang)) {
                            if (_dicByLangAndView.ContainsKey(lang)) {
                                var dic = _dicByLangAndView[lang];
                                if (dic.ContainsKey(entity.ViewId) && dic[entity.ViewId].Contains(entity)) {
                                    dic[entity.ViewId].Remove(entity);
                                    if (dic[entity.ViewId].Count == 0) {
                                        dic.Remove(entity.ViewId);
                                    }
                                }
                                if (_dicByLangAndView.Count == 0) {
                                    _dicByLangAndView.Remove(lang);
                                }
                            }
                        }
                        var repository = Repository.CreateLanguageRepository<LangItem>();
                        repository.Remove(entity.Id);
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
                    IRepository<LangItem> repository = Repository.CreateLanguageRepository<LangItem>();
                    IList<LangItem> langItems = repository.GetAll();
                    langItems = new List<LangItem> {
                        new LangItem {
                                LangId = new Guid("9DD5F05A-003E-4A9D-91CA-38548D960BD4"),
                                ViewId = "LoginWindow",
                                Key = "Title",
                                Value = "登录"
                            },
                            new LangItem {
                                LangId = new Guid("9DD5F05A-003E-4A9D-91CA-38548D960BD4"),
                                ViewId = "LoginWindow",
                                Key = "LblHost",
                                Value = "服务器："
                            },
                            new LangItem {
                                LangId = new Guid("9DD5F05A-003E-4A9D-91CA-38548D960BD4"),
                                ViewId = "LoginWindow",
                                Key = "LblLoginName",
                                Value = "登录名："
                            },
                            new LangItem {
                                LangId = new Guid("9DD5F05A-003E-4A9D-91CA-38548D960BD4"),
                                ViewId = "LoginWindow",
                                Key = "LblPassword",
                                Value = "密码："
                            },new LangItem {
                                LangId = new Guid("584398F7-5BDD-41B3-94C6-50A14F23DB71"),
                                ViewId = "LoginWindow",
                                Key = "Title",
                                Value = "login"
                            },
                            new LangItem {
                                LangId = new Guid("584398F7-5BDD-41B3-94C6-50A14F23DB71"),
                                ViewId = "LoginWindow",
                                Key = "LblHost",
                                Value = "server："
                            },
                            new LangItem {
                                LangId = new Guid("584398F7-5BDD-41B3-94C6-50A14F23DB71"),
                                ViewId = "LoginWindow",
                                Key = "LblLoginName",
                                Value = "login name："
                            },
                            new LangItem {
                                LangId = new Guid("584398F7-5BDD-41B3-94C6-50A14F23DB71"),
                                ViewId = "LoginWindow",
                                Key = "LblPassword",
                                Value = "password："
                            }
                    };
                    foreach (var lang in LangSet.Instance) {
                        var dic = new Dictionary<string, List<ILangItem>>();
                        _dicByLangAndView.Add(lang, dic);
                        foreach (var item in langItems.Where(a => a.LangId == lang.GetId())) {
                            if (!dic.ContainsKey(item.ViewId)) {
                                dic.Add(item.ViewId, new List<ILangItem>());
                            }
                            dic[item.ViewId].Add(item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public List<ILangItem> GetLangItems(ILang lang, string viewId) {
            InitOnece();
            if (!_dicByLangAndView.ContainsKey(lang)) {
                return new List<ILangItem>();
            }
            var dic = _dicByLangAndView[lang];
            if (!dic.ContainsKey(viewId)) {
                return new List<ILangItem>();
            }
            return dic[viewId].Cast<ILangItem>().ToList();
        }

        public Dictionary<string, List<ILangItem>> GetLangItems(ILang lang) {
            InitOnece();
            if (!_dicByLangAndView.ContainsKey(lang)) {
                return new Dictionary<string, List<ILangItem>>();
            }
            return _dicByLangAndView[lang];
        }
    }
}
