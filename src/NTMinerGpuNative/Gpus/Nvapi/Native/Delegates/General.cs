using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Helpers;
using NTMiner.Gpus.Nvapi.Native.Helpers.Structures;

// ReSharper disable InconsistentNaming

namespace NTMiner.Gpus.Nvapi.Native.Delegates
{
    internal static class General
    {
        [FunctionId(FunctionId.NvAPI_GetErrorMessage)]
        public delegate Status NvAPI_GetErrorMessage([In] Status status, out ShortString message);

        [FunctionId(FunctionId.NvAPI_GetInterfaceVersionString)]
        public delegate Status NvAPI_GetInterfaceVersionString(out ShortString version);

        [FunctionId(FunctionId.NvAPI_Initialize)]
        public delegate Status NvAPI_Initialize();


        [FunctionId(FunctionId.NvAPI_RestartDisplayDriver)]
        public delegate Status NvAPI_RestartDisplayDriver();

        [FunctionId(FunctionId.NvAPI_SYS_GetChipSetInfo)]
        public delegate Status NvAPI_SYS_GetChipSetInfo(
            [In] [Accepts(typeof(ChipsetInfoV4), typeof(ChipsetInfoV3), typeof(ChipsetInfoV2), typeof(ChipsetInfoV1))]
            ValueTypeReference chipsetInfo);

        [FunctionId(FunctionId.NvAPI_SYS_GetDriverAndBranchVersion)]
        public delegate Status NvAPI_SYS_GetDriverAndBranchVersion(
            out uint driverVersion,
            out ShortString buildBranchString);

        [FunctionId(FunctionId.NvAPI_SYS_GetLidAndDockInfo)]
        public delegate Status NvAPI_SYS_GetLidAndDockInfo([In] [Out] ref LidDockParameters lidAndDock);

        [FunctionId(FunctionId.NvAPI_Unload)]
        public delegate Status NvAPI_Unload();
    }
}