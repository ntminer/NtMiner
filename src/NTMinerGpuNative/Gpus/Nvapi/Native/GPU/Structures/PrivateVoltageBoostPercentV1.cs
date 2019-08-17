using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Helpers;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU voltage boost percentage
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateVoltageBoostPercentV1 : IInitializable
    {
        internal const int MaxNumberOfUnknown = 8;

        internal StructureVersion _Version;

        internal readonly uint _Percent;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown)]
        internal readonly uint[] _Unknown;

        /// <summary>
        ///     Gets the voltage boost in percentage
        /// </summary>
        public uint Percent
        {
            get => _Percent;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PrivateVoltageBoostPercentV1" />
        /// </summary>
        /// <param name="percent">The voltage boost in percentage</param>
        public PrivateVoltageBoostPercentV1(uint percent)
        {
            this = typeof(PrivateVoltageBoostPercentV1).Instantiate<PrivateVoltageBoostPercentV1>();
            _Percent = percent;
        }
    }
}