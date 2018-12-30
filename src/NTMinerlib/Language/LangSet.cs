using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language {
    public class LangSet : IEnumerable<ILang> {
        public static readonly LangSet Instance = new LangSet();

        private readonly List<ILang> _langs = new List<ILang>() {
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

        private LangSet() { }

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
