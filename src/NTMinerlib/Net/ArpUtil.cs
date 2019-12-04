using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NTMiner.Net {
    public static class ArpUtil {
        // The max number of physical addresses.
        const int MAXLEN_PHYSADDR = 8;

        // Define the MIB_IPNETROW structure.
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_IPNETROW {
            [MarshalAs(UnmanagedType.U4)]
            public int dwIndex;
            [MarshalAs(UnmanagedType.U4)]
            public int dwPhysAddrLen;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac0;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac1;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac2;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac3;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac4;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac5;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac6;
            [MarshalAs(UnmanagedType.U1)]
            public byte mac7;
            [MarshalAs(UnmanagedType.U4)]
            public int dwAddr;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        // Declare the GetIpNetTable function.
        [DllImport("IpHlpApi.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int GetIpNetTable(
           IntPtr pIpNetTable,
           [MarshalAs(UnmanagedType.U4)]
         ref int pdwSize,
           bool bOrder);

        [DllImport("IpHlpApi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int FreeMibTable(IntPtr plpNetTable);

        // The insufficient buffer error.
        const int ERROR_INSUFFICIENT_BUFFER = 122;

        private static bool _isFirst = true;
        public static string GetIpByMac(string mac) {
            if (_isFirst) {
                _isFirst = false;
                Parallel.ForEach(VirtualRoot.LocalIpSet.GetAllSubnetIps(), a => { DeviceScanner.IsHostAccessible(a); });
            }
            // The number of bytes needed.
            int bytesNeeded = 0;

            // The result from the API call.
            int result = GetIpNetTable(IntPtr.Zero, ref bytesNeeded, false);

            // Call the function, expecting an insufficient buffer.
            if (result != ERROR_INSUFFICIENT_BUFFER) {
                return string.Empty;
            }

            // Allocate the memory, do it in a try/finally block, to ensure
            // that it is released.
            IntPtr buffer = IntPtr.Zero;

            // Try/finally.
            try {
                // Allocate the memory.
                buffer = Marshal.AllocCoTaskMem(bytesNeeded);

                // Make the call again. If it did not succeed, then
                // raise an error.
                result = GetIpNetTable(buffer, ref bytesNeeded, false);

                // If the result is not 0 (no error), then throw an exception.
                if (result != 0) {
                    return string.Empty;
                }

                // Now we have the buffer, we have to marshal it. We can read
                // the first 4 bytes to get the length of the buffer.
                int entries = Marshal.ReadInt32(buffer);

                // Increment the memory pointer by the size of the int.
                IntPtr currentBuffer = new IntPtr(buffer.ToInt64() +
                   Marshal.SizeOf(typeof(int)));

                // Cycle through the entries.
                for (int index = 0; index < entries; index++) {
                    // Call PtrToStructure, getting the structure information.
                    var row = (MIB_IPNETROW)Marshal.PtrToStructure(new IntPtr(currentBuffer.ToInt64() + (index * Marshal.SizeOf(typeof(MIB_IPNETROW)))), typeof(MIB_IPNETROW));
                    string ttMAC = $"{row.mac0.ToString("X2")}:{row.mac1.ToString("X2")}:{row.mac2.ToString("X2")}:{row.mac3.ToString("X2")}:{row.mac4.ToString("X2")}:{row.mac5.ToString("X2")}";
                    if (string.Equals(mac, ttMAC, StringComparison.OrdinalIgnoreCase)) {
                        return new IPAddress(BitConverter.GetBytes(row.dwAddr)).ToString();
                    }
                    Console.WriteLine(new IPAddress(BitConverter.GetBytes(row.dwAddr)).ToString() + " " + ttMAC);
                }
                return string.Empty;
            }
            finally {
                // Release the memory.
                FreeMibTable(buffer);
            }
        }
    }
}
