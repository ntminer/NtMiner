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

        public IEnumerator<ILang> GetEnumerator() {
            return _langs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _langs.GetEnumerator();
        }
    }
}
