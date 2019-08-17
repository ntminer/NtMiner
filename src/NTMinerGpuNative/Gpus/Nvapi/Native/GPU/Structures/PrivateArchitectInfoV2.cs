using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding a GPU architecture
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [StructureVersion(2)]
    public struct PrivateArchitectInfoV2 : IInitializable
    {
        internal StructureVersion _Version;
        internal uint _Unknown1;
        internal uint _Unknown2;
        internal uint _Revision;

        /// <summary>
        ///     Gets the GPU revision
        /// </summary>
        public uint Revision
        {
            get => _Revision;
        }
    }
}