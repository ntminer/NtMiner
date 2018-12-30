using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language {
    public class LangItemSet {
        public static readonly LangItemSet Instance = new LangItemSet();

        private readonly Dictionary<ILang, Dictionary<string, List<ILangItem>>> _dicByLangAndView = new Dictionary<ILang, Dictionary<string, List<ILangItem>>>();

        private LangItemSet() { }

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
            return dic[viewId];
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
