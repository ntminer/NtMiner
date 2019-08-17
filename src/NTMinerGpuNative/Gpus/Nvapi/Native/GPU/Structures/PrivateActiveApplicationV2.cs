using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    [StructureVersion(2)]
    [StructLayout(LayoutKind.Sequential)]
    public struct PrivateActiveApplicationV2 : IInitializable
    {
        internal const int MaximumNumberOfApplications = 128;

        internal StructureVersion _Version;
        internal uint _ProcessId;
        internal LongString _ProcessName;

        public int ProcessId
        {
            get => (int) _ProcessId;
        }

        public string ProcessName
        {
            get => _ProcessName.Value;
        }
    }
}