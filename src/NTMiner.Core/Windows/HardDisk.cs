using System.Runtime.InteropServices;

namespace NTMiner.Windows {
    /// <summary>
    /// Class for getting info for HardDisks
    /// </summary>
    public class HardDisk {
        #region P/Invokes

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
                out ulong lpFreeBytesAvailable,
                out ulong lpTotalNumberOfBytes,
                out ulong lpTotalNumberOfFreeBytes);

        #endregion

        #region Methods

        /// <summary>
        /// Gets the space of a drive according to the DiskSpaceFlag set.
        /// </summary>
        /// <param name="driveLetter">The drive letter in the form : "C:\\" or @"C:\"</param>
        /// <param name="flags">The specified disk flag for how the diskspace is retrieved</param>
        /// <returns>The diskspace in bytes as an unsigned long</returns>
        public static ulong GetFreeSpace(string driveLetter, DiskSpaceFlags flags) {
            ulong freeBytesAvailable;
            ulong totalNumberOfBytes;
            ulong totalNumberOfFreeBytes;

            GetDiskFreeSpaceEx(driveLetter, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);

            switch (flags) {
                case DiskSpaceFlags.FreeBytesAvailable:
                    return freeBytesAvailable;

                case DiskSpaceFlags.TotalNumberOfBytes:
                    return totalNumberOfBytes;

                case DiskSpaceFlags.TotalNumberOfFreeBytes:
                    return totalNumberOfFreeBytes;
            }

            return 0;
        }

        #endregion
    }
}
