using System.Runtime.InteropServices;

namespace HardwareProviders.Internals {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IOControlCode {
        private readonly uint code;

        public IOControlCode(uint deviceType, uint function, Access access) :
            this(deviceType, function, Method.Buffered, access) {
        }

        public IOControlCode(uint deviceType, uint function, Method method,
            Access access) {
            code = (deviceType << 16) |
                   ((uint)access << 14) | (function << 2) | (uint)method;
        }

        public enum Method : uint {
            Buffered = 0,
            InDirect = 1,
            OutDirect = 2,
            Neither = 3
        }

        public enum Access : uint {
            Any = 0,
            Read = 1,
            Write = 2
        }
    }
}