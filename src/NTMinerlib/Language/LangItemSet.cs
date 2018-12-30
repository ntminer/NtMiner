using NTMiner.Language.Impl;
using System.Collections.Generic;

namespace NTMiner.Language {
    public class LangItemSet {
        public static readonly LangItemSet Instance = new LangItemSet();

        private readonly Dictionary<string, Dictionary<string, List<ILangItem>>> _dicByLangAndView = new Dictionary<string, Dictionary<string, List<ILangItem>>>() {
            {
                "cn-zh", new Dictionary<string, List<ILangItem>> {
                    {
                        "LoginWindow", new List<ILangItem> {
                            new LangItem {
                                LangCode = "cn-zh",
                                ViewId = "LoginWindow",
                                Key = "Title",
                                Value = "登录"
                            },
                            new LangItem {
                                LangCode = "cn-zh",
                                ViewId = "LoginWindow",
                                Key = "HostLbl",
                                Value = "服务器："
                            },
                            new LangItem {
                                LangCode = "cn-zh",
                                ViewId = "LoginWindow",
                                Key = "LoginNameLbl",
                                Value = "登录名："
                            },
                            new LangItem {
                                LangCode = "cn-zh",
                                ViewId = "LoginWindow",
                                Key = "PasswordLbl",
                                Value = "密码："
                            }
                        }
                    }
                }
            },
            {
                "en", new Dictionary<string, List<ILangItem>> {
                    {
                        "LoginWindow", new List<ILangItem> {
                            new LangItem {
                                LangCode = "en",
                                ViewId = "LoginWindow",
                                Key = "Title",
                                Value = "login"
                            },
                            new LangItem {
                                LangCode = "en",
                                ViewId = "LoginWindow",
                                Key = "HostLbl",
                                Value = "server："
                            },
                            new LangItem {
                                LangCode = "en",
                                ViewId = "LoginWindow",
                                Key = "LoginNameLbl",
                                Value = "login name："
                            },
                            new LangItem {
                                LangCode = "en",
                                ViewId = "LoginWindow",
                                Key = "PasswordLbl",
                                Value = "password："
                            }
                        }
                    }
                }
            }
        };

        private LangItemSet() { }

        public List<ILangItem> GetLangItems(string lang, string viewId) {
            if (!_dicByLangAndView.ContainsKey(lang)) {
                return new List<ILangItem>();
            }
            var dic = _dicByLangAndView[lang];
            if (!dic.ContainsKey(viewId)) {
                return new List<ILangItem>();
            }
            return dic[viewId];
        }
    }
}
