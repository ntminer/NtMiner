using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.General.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct UnicodeString : IInitializable
    {
        public const int UnicodeStringLength = 2048;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UnicodeStringLength)]
        private readonly string _Value;

        public string Value
        {
            get => _Value;
        }

        public UnicodeString(string value)
        {
            _Value = value ?? string.Empty;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}