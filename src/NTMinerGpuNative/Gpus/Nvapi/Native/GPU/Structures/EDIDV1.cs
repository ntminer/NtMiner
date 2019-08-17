using System;
using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Helpers;
using NTMiner.Gpus.Nvapi.Native.Interfaces;
using NTMiner.Gpus.Nvapi.Native.Interfaces.GPU;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    /// <summary>
    ///     Holds whole or a part of the EDID information
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct EDIDV1 : IEDID, IInitializable
    {
        /// <summary>
        ///     The maximum number of data bytes that this structure can hold
        /// </summary>
        public const int MaxDataSize = 256;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDataSize)]
        internal byte[] _Data;

        internal static EDIDV1 CreateWithData(byte[] data)
        {
            if (data.Length > MaxDataSize)
            {
                throw new ArgumentException("Data is too big.", nameof(data));
            }

            var edid = typeof(EDIDV1).Instantiate<EDIDV1>();
            Array.Copy(data, edid._Data, data.Length);

            return edid;
        }

        /// <summary>
        ///     Gets whole or a part of the EDID data
        /// </summary>
        public byte[] Data
        {
            get => _Data;
        }
    }
}