using System;

namespace NTMiner.ServiceContracts.DataObjects {
    public interface INTMinerFile {
        Guid Id { get; }

        string FileName { get; }

        string Version { get; }

        string VersionTag { get; }

        DateTime CreatedOn { get; }

        DateTime PublishOn { get; }
    }
}
