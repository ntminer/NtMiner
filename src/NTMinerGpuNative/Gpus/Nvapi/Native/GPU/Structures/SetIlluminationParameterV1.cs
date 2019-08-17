using System.Runtime.InteropServices;
using NTMiner.Gpus.Nvapi.Native.Attributes;
using NTMiner.Gpus.Nvapi.Native.General.Structures;
using NTMiner.Gpus.Nvapi.Native.Helpers;
using NTMiner.Gpus.Nvapi.Native.Interfaces;

namespace NTMiner.Gpus.Nvapi.Native.GPU.Structures
{
    /// <summary>
    ///     Holds necessary information to set an illumination attribute value
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct SetIlluminationParameterV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal PhysicalGPUHandle _GPUHandle;
        internal IlluminationAttribute _Attribute;
        internal uint _ValueInPercentage;

        /// <summary>
        ///     Creates a new instance of <see cref="SetIlluminationParameterV1" />.
        /// </summary>
        /// <param name="gpuHandle">The physical gpu handle.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="valueInPercentage">The attribute value in percentage.</param>
        public SetIlluminationParameterV1(
            PhysicalGPUHandle gpuHandle,
            IlluminationAttribute attribute,
            uint valueInPercentage)
        {
            this = typeof(SetIlluminationParameterV1).Instantiate<SetIlluminationParameterV1>();
            _GPUHandle = gpuHandle;
            _Attribute = attribute;
            _ValueInPercentage = valueInPercentage;
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