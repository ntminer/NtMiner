using System;

namespace NTMiner.Core {
    public interface ICaptcha {
        Guid Id { get; }
        string Code { get; }
        DateTime CreatedOn { get; }
        string Ip { get; }
    }
}
