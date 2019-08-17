using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Interfaces;
using NTMiner.Gpus.Nvapi.Native.Interfaces.GPU;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    /// <summary>
    ///     Holds clock frequencies currently associated with a physical GPU
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ClockFrequenciesV1 : IInitializable, IClockFrequencies
    {
        internal const int MaxClocksPerGPU = 32;

        internal StructureVersion _Version;
        internal readonly uint _Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxClocksPerGPU)]
        internal ClockDomainInfo[] _Clocks;

        /// <inheritdoc />
        public IDictionary<PublicClockDomain, ClockDomainInfo> Clocks
        {
            get => _Clocks
                .Select((value, index) => new {index, value})
                .Where(arg => Enum.IsDefined(typeof(PublicClockDomain), arg.index))
                .ToDictionary(arg => (PublicClockDomain) arg.index, arg => arg.value);
        }

        /// <inheritdoc />
        public ClockType ClockType
        {
            get => ClockType.CurrentClock;
        }

        /// <inheritdoc />
        public ClockDomainInfo GraphicsClock
        {
            get => _Clocks[(int) PublicClockDomain.Graphics];
        }

        /// <inheritdoc />
        public ClockDomainInfo MemoryClock
        {
            get => _Clocks[(int) PublicClockDomain.Memory];
        }

        /// <inheritdoc />
        public ClockDomainInfo VideoDecodingClock
        {
            get => _Clocks[(int) PublicClockDomain.Video];
        }

        /// <inheritdoc />
        public ClockDomainInfo ProcessorClock
        {
            get => _Clocks[(int) PublicClockDomain.Processor];
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{ClockType}] 3D Graphics = {GraphicsClock} - Memory = {MemoryClock} - Video Decoding = {VideoDecodingClock} - Processor = {ProcessorClock}";
        }
    }
}