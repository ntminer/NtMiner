using System;

namespace NTMiner.FileETag {
    public interface IETag {
        string Key { get; }
        string Value { get; }
        DateTime TimeStamp { get; }
    }
}
