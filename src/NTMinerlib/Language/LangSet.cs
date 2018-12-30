using System.Collections;
using System.Collections.Generic;
using NTMiner.Language.Impl;

namespace NTMiner.Language {
    public class LangSet : IEnumerable<ILang> {
        public static readonly LangSet Instance = new LangSet();

        private readonly List<ILang> _langs = new List<ILang>() {
            new Lang {
                Code = "en",
                Name = "english"
            },
            new Lang {
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
                    
                    _isInited = true;
                }
            }
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
