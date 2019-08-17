using System;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi.Native.Display.Structures
{
    /// <summary>
    ///     Holds VESA scan out timing parameters
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Timing : IEquatable<Timing>
    {
        internal readonly ushort _HorizontalVisible;
        internal readonly ushort _HorizontalBorder;
        internal readonly ushort _HorizontalFrontPorch;
        internal readonly ushort _HorizontalSyncWidth;
        internal readonly ushort _HorizontalTotal;
        internal readonly TimingHorizontalSyncPolarity _HorizontalSyncPolarity;
        internal readonly ushort _VerticalVisible;
        internal readonly ushort _VerticalBorder;
        internal readonly ushort _VerticalFrontPorch;
        internal readonly ushort _VerticalSyncWidth;
        internal readonly ushort _VerticalTotal;
        internal readonly TimingVerticalSyncPolarity _VerticalSyncPolarity;
        internal readonly TimingScanMode _ScanMode;
        internal readonly uint _PixelClockIn10KHertz;
        internal readonly TimingExtra _Extra;

        /// <inheritdoc />
        public bool Equals(Timing other)
        {
            return _HorizontalVisible == other._HorizontalVisible &&
                   _HorizontalBorder == other._HorizontalBorder &&
                   _HorizontalFrontPorch == other._HorizontalFrontPorch &&
                   _HorizontalSyncWidth == other._HorizontalSyncWidth &&
                   _HorizontalTotal == other._HorizontalTotal &&
                   _HorizontalSyncPolarity == other._HorizontalSyncPolarity &&
                   _VerticalVisible == other._VerticalVisible &&
                   _VerticalBorder == other._VerticalBorder &&
                   _VerticalFrontPorch == other._VerticalFrontPorch &&
                   _VerticalSyncWidth == other._VerticalSyncWidth &&
                   _VerticalTotal == other._VerticalTotal &&
                   _VerticalSyncPolarity == other._VerticalSyncPolarity &&
                   _ScanMode == other._ScanMode &&
                   _PixelClockIn10KHertz == other._PixelClockIn10KHertz &&
                   _Extra.Equals(other._Extra);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Timing && Equals((Timing) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _HorizontalVisible.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalBorder.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalFrontPorch.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalSyncWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ _HorizontalTotal.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _HorizontalSyncPolarity;
                hashCode = (hashCode * 397) ^ _VerticalVisible.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalBorder.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalFrontPorch.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalSyncWidth.GetHashCode();
                hashCode = (hashCode * 397) ^ _VerticalTotal.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) _VerticalSyncPolarity;
                hashCode = (hashCode * 397) ^ (int) _ScanMode;
                hashCode = (hashCode * 397) ^ (int) _PixelClockIn10KHertz;
                hashCode = (hashCode * 397) ^ _Extra.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        ///     Horizontal visible
        /// </summary>
        public int HorizontalVisible
        {
            get => _HorizontalVisible;
        }

        /// <summary>
        ///     Horizontal border
        /// </summary>
        public int HorizontalBorder
        {
            get => _HorizontalBorder;
        }

        /// <summary>
        ///     Horizontal front porch
        /// </summary>
        public int HorizontalFrontPorch
        {
            get => _HorizontalFrontPorch;
        }

        /// <summary>
        ///     Horizontal sync width
        /// </summary>
        public int HorizontalSyncWidth
        {
            get => _HorizontalSyncWidth;
        }

        /// <summary>
        ///     Horizontal total
        /// </summary>
        public int HorizontalTotal
        {
            get => _HorizontalTotal;
        }

        /// <summary>
        ///     Horizontal sync polarity
        /// </summary>
        public TimingHorizontalSyncPolarity HorizontalSyncPolarity
        {
            get => _HorizontalSyncPolarity;
        }

        /// <summary>
        ///     Vertical visible
        /// </summary>
        public int VerticalVisible
        {
            get => _VerticalVisible;
        }

        /// <summary>
        ///     Vertical border
        /// </summary>
        public int VerticalBorder
        {
            get => _VerticalBorder;
        }

        /// <summary>
        ///     Vertical front porch
        /// </summary>
        public int VerticalFrontPorch
        {
            get => _VerticalFrontPorch;
        }

        /// <summary>
        ///     Vertical sync width
        /// </summary>
        public int VerticalSyncWidth
        {
            get => _VerticalSyncWidth;
        }

        /// <summary>
        ///     Vertical total
        /// </summary>
        public int VerticalTotal
        {
            get => _VerticalTotal;
        }

        /// <summary>
        ///     Vertical sync polarity
        /// </summary>
        public TimingVerticalSyncPolarity VerticalSyncPolarity
        {
            get => _VerticalSyncPolarity;
        }

        /// <summary>
        ///     Scan mode
        /// </summary>
        public TimingScanMode ScanMode
        {
            get => _ScanMode;
        }

        /// <summary>
        ///     Pixel clock in 10 kHz
        /// </summary>
        public int PixelClockIn10KHertz
        {
            get => (int) _PixelClockIn10KHertz;
        }

        /// <summary>
        ///     Other timing related extras
        /// </summary>
        public TimingExtra Extra
        {
            get => _Extra;
        }
    }
}