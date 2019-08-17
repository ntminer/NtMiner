using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Helpers;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    /// <summary>
    ///     Holds necessary information to get an illumination attribute value
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct GetIlluminationParameterV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal PhysicalGPUHandle _GPUHandle;
        internal IlluminationAttribute _Attribute;
        internal uint _ValueInPercentage;

        /// <summary>
        ///     Creates a new instance of <see cref="GetIlluminationParameterV1" />.
        /// </summary>
        /// <param name="gpuHandle">The physical gpu handle.</param>
        /// <param name="attribute">The attribute.</param>
        public GetIlluminationParameterV1(PhysicalGPUHandle gpuHandle, IlluminationAttribute attribute)
        {
            this = typeof(GetIlluminationParameterV1).Instantiate<GetIlluminationParameterV1>();
            _GPUHandle = gpuHandle;
            _Attribute = attribute;
        }

        /// <summary>
        ///     Gets the parameter physical gpu handle
        /// </summary>
        public PhysicalGPUHandle PhysicalGPUHandle
        {
            get => _GPUHandle;
        }

        /// <summary>
        ///     Gets the parameter attribute
        /// </summary>
        public IlluminationAttribute Attribute
        {
            get => _Attribute;
        }

        /// <summary>
        ///     Gets the parameter value in percentage
        /// </summary>
        public uint ValueInPercentage
        {
            get => _ValueInPercentage;
        }
    }
}