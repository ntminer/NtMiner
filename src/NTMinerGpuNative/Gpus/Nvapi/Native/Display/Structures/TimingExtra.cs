using System;
using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.Display.Structures
{
    /// <summary>
    ///     Holds NVIDIA-specific timing extras
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct TimingExtra : IInitializable, IEquatable<TimingExtra>
    {
        internal readonly uint _HardwareFlags;
        internal readonly ushort _RefreshRate;
        internal readonly uint _FrequencyInMillihertz;
        internal readonly ushort _VerticalAspect;
        internal readonly ushort _HorizontalAspect;
        internal readonly ushort _PixelRepetition;
        internal readonly uint _Status;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        internal string _Name;

        /// <inheritdoc />
        public bool Equals(TimingExtra other)
        {
            return _HardwareFlags == other._HardwareFlags &&
                   _RefreshRate == other._RefreshRate &&
                   _FrequencyInMillihertz == other._FrequencyInMillihertz &&
                   _VerticalAspect == other._VerticalAspect &&
                   _HorizontalAspect == other._HorizontalAspect &&
                   _PixelRepetition == other._PixelRepetition &&
                   _Status == other._Status;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TimingExtra extra && Equals(extra);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _HardwareFlags;
                hashCode = (hashCode * 397) ^ _RefreshRate.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _FrequencyInMillihertz;
                hashCode = (hashCode * 397) ^ _VerticalAspect.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalAspect.GetHashCode();
                hashCode = (hashCode * 397) ^ _PixelRepetition.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _Status;

                return hashCode;
            }
        }

        /// <summary>
        ///     Reserved for NVIDIA hardware-based enhancement, such as double-scan.
        /// </summary>
        public uint HardwareFlags
        {
            get => _HardwareFlags;
        }

        /// <summary>
        ///     Logical refresh rate to present
        /// </summary>
        public int RefreshRate
        {
            get => _RefreshRate;
        }

        /// <summary>
        ///     Physical vertical refresh rate in 0.001Hz
        /// </summary>
        public int FrequencyInMillihertz
        {
            get => (int) _FrequencyInMillihertz;
        }

        /// <summary>
        ///     Display vertical aspect
        /// </summary>
        public int VerticalAspect
        {
            get => _VerticalAspect;
        }

        /// <summary>
        ///     Display horizontal aspect
        /// </summary>
        public int HorizontalAspect
        {
            get => _HorizontalAspect;
        }

        /// <summary>
        ///     Bit-wise pixel repetition factor: 0x1:no pixel repetition; 0x2:each pixel repeats twice horizontally,..
        /// </summary>
        public int PixelRepetition
        {
            get => _PixelRepetition;
        }

        /// <summary>
        ///     Timing standard
        /// </summary>
        public uint Status
        {
            get => _Status;
        }

        /// <summary>
        ///     Timing name
        /// </summary>
        public string Name
        {
            get => _Name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}