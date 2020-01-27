using System;

namespace NTMiner.Core.MinerServer {
    public interface INTMinerFile {
        Guid Id { get; }

        NTMinerAppType AppType { get; }

        string FileName { get; }

        string Version { get; }

        string VersionTag { get; }

        DateTime CreatedOn { get; }

        DateTime PublishOn { get; }

        string Title { get; }

        string Description { get; }
    }
}
