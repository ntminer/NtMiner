using System;

namespace NTMiner.Gpus.Nvapi.Native.Exceptions
{
    /// <summary>
    ///     Represents errors that raised by NvAPIWrapper
    /// </summary>
    public class NVIDIANotSupportedException : NotSupportedException
    {
        internal NVIDIANotSupportedException(string message) : base(message)
        {
        }
    }
}