using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace NTMiner.Ip.Impl {
    public class IpSet : IIpSet {
        public static readonly IIpSet Instance = new IpSet();

        private readonly HashSet<IPAddress> _hashSet = new HashSet<IPAddress>();

        private IpSet() { }

        private object _locker = new object();
        private DateTime _lastInitOn = DateTime.MinValue;

        private void InitOnece() {
            DateTime now = DateTime.Now;
            if (_lastInitOn.AddMinutes(1) < now) {
                lock (_locker) {
                    if (_lastInitOn.AddMinutes(1) < now) {
                        foreach (var ip in GetAllIpsOnlan()) {
                            _hashSet.Add(ip);
                        }
                        _lastInitOn = DateTime.Now;
                    }
                }
            }
        }

        public bool Contains(IPAddress ip) {
            InitOnece();
            return _hashSet.Contains(ip);
        }

        public IEnumerator<IPAddress> GetEnumerator() {
            InitOnece();
            return _hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _hashSet.GetEnumerator();
        }

        /// <summary>
        /// MIB_IPNETROW structure returned by GetIpNetTable
        /// DO NOT MODIFY THIS STRUCTURE.
        /// </summary>
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

        /// <summary>
        /// GetIpNetTable external method
        /// </summary>
        /// <param name="pIpNetTable"></param>
        /// <param name="pdwSize"></param>
        /// <param name="bOrder"></param>
        /// <returns></returns>
        [DllImport("IpHlpApi.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int GetIpNetTable(IntPtr pIpNetTable,
                                        [MarshalAs(UnmanagedType.U4)] ref int pdwSize, bool bOrder);

        /// <summary>
        /// Get the IP and MAC addresses of all known devices on the LAN
        /// </summary>
        /// <remarks>
        /// 1) This table is not updated often - it can take some human-scale time
        ///    to notice that a device has dropped off the network, or a new device
        ///    has connected.
        /// 2) This discards non-local devices if they are found - these are multicast
        ///    and can be discarded by IP address range.
        /// </remarks>
        /// <returns></returns>
        private static HashSet<IPAddress> GetAllIpsOnlan() {
            HashSet<IPAddress> all = new HashSet<IPAddress> {
                IPAddress.Parse("127.0.0.1")
            };
            int spaceForNetTable = 0;
            // Get the space needed
            // We do that by requesting the table, but not giving any space at all.
            // The return value will tell us how much we actually need.
            GetIpNetTable(IntPtr.Zero, ref spaceForNetTable, false);
            // Allocate the space
            // We use a try-finally block to ensure release.
            IntPtr rawTable = IntPtr.Zero;
            try {
                rawTable = Marshal.AllocCoTaskMem(spaceForNetTable);
                // Get the actual data
                int errorCode = GetIpNetTable(rawTable, ref spaceForNetTable, false);
                if (errorCode != 0) {
                    // Failed for some reason - can do no more here.
                    throw new Exception(string.Format(
                        "Unable to retrieve network table. Error code {0}", errorCode));
                }
                // Get the rows count
                int rowsCount = Marshal.ReadInt32(rawTable);
                IntPtr currentBuffer = new IntPtr(rawTable.ToInt64() + Marshal.SizeOf(typeof(Int32)));
                // Convert the raw table to individual entries
                MIB_IPNETROW[] rows = new MIB_IPNETROW[rowsCount];
                for (int index = 0; index < rowsCount; index++) {
                    rows[index] = (MIB_IPNETROW)Marshal.PtrToStructure(new IntPtr(currentBuffer.ToInt64() +
                                                                                  (index * Marshal.SizeOf(typeof(MIB_IPNETROW)))
                                                                                 ),
                                                                       typeof(MIB_IPNETROW));
                }
                PhysicalAddress virtualMAC = new PhysicalAddress(new byte[] { 0, 0, 0, 0, 0, 0 });
                PhysicalAddress broadcastMAC = new PhysicalAddress(new byte[] { 255, 255, 255, 255, 255, 255 });
                foreach (MIB_IPNETROW row in rows) {
                    IPAddress ip = new IPAddress(BitConverter.GetBytes(row.dwAddr));
                    byte[] rawMAC = new byte[] { row.mac0, row.mac1, row.mac2, row.mac3, row.mac4, row.mac5 };
                    PhysicalAddress pa = new PhysicalAddress(rawMAC);
                    if (!pa.Equals(virtualMAC) && !pa.Equals(broadcastMAC) && !IsMulticast(ip)) {
                        all.Add(ip);
                    }
                }
            }
            finally {
                // Release the memory.
                Marshal.FreeCoTaskMem(rawTable);
            }
            return all;
        }

        /// <summary>
        /// Returns true if the specified IP address is a multicast address
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsMulticast(IPAddress ip) {
            bool result = true;
            if (!ip.IsIPv6Multicast) {
                byte highIP = ip.GetAddressBytes()[0];
                if (highIP < 224 || highIP > 239) {
                    result = false;
                }
            }
            return result;
        }
    }
}
