using System;
using NTMiner.Gpus.Nvapi.Native.Helpers;

namespace NTMiner.Gpus.Nvapi.Native.Attributes
{
    [AttributeUsage(AttributeTargets.Delegate)]
    internal class FunctionIdAttribute : Attribute
    {
        public FunctionIdAttribute(FunctionId functionId)
        {
            FunctionId = functionId;
        }

        public FunctionId FunctionId { get; set; }
    }
}