using System;
using System.Runtime.InteropServices;

namespace HardwareProviders.Board
{
    static class SystemCall
    {
        private const string KERNEL = "kernel32.dll";

        [DllImport(KERNEL, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int EnumSystemFirmwareTables(FirmwareTable.Provider firmwareTableProviderSignature, IntPtr firmwareTableBuffer, int bufferSize);

        [DllImport(KERNEL, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int GetSystemFirmwareTable(FirmwareTable.Provider firmwareTableProviderSignature, int firmwareTableID, IntPtr firmwareTableBuffer, int bufferSize);
    }
}
