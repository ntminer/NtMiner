using NTMiner.Gpus.Nvapi.Native.GPU;

namespace NTMiner.Gpus.Nvapi.Native.Interfaces.GPU
{
    /// <summary>
    ///     Holds information regarding a clock domain of a performance state
    /// </summary>
    public interface IPerformanceStatesClock
    {
        /// <summary>
        ///     Gets the clock domain identification
        /// </summary>
        PublicClockDomain DomainId { get; }

        /// <summary>
        ///     Gets the clock frequency in kHz
        /// </summary>
        uint Frequency { get; }
    }
}