using System;

namespace NTMiner.Gpus.Nvapi.Native.Interfaces
{
    /// <summary>
    ///     Marker interface for all types that should be allocated before passing to the managed code
    /// </summary>
    internal interface IAllocatable : IInitializable, IDisposable
    {
        void Allocate();
    }
}