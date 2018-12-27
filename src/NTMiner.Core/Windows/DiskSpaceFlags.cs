
namespace NTMiner.Windows {
    /// <summary>
    /// Flags associated for use with GetFreeSpace
    /// </summary>
    public enum DiskSpaceFlags : ulong {
        /// <summary>
        /// Receive the total number of free bytes on a disk that are 
        /// available to the user who is associated with the calling thread.
        /// 
        /// <remarks>
        /// The returned value can be null.
        /// </remarks>
        /// 
        /// <remarks>
        /// If per-user quotas are being used, this value may be 
        /// less than the total number of free bytes on a disk.
        /// </remarks>
        /// </summary>
        FreeBytesAvailable = 1,

        /// <summary>
        /// Receive the total number of bytes on a disk that are 
        /// available to the user who is associated with the calling thread.
        /// 
        /// <remarks>
        /// The value returned can be null
        /// </remarks>
        /// 
        /// <remarks>
        /// If per-user quotas are being used, this value may be 
        /// less than the total number of free bytes on a disk.
        /// </remarks>
        /// </summary>
        TotalNumberOfBytes = 2,

        /// <summary>
        /// Receive the total number of free bytes on a disk.
        /// 
        /// <remarks>The value returned can be null</remarks>
        /// </summary>
        TotalNumberOfFreeBytes = 3,
    };
}
