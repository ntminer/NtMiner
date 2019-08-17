using System;
using NTMiner.Gpus.Nvapi.Native.General;

namespace NTMiner.Gpus.Nvapi.Native.Exceptions
{
    /// <summary>
    ///     Represents errors that raised by NVIDIA Api
    /// </summary>
    public class NVIDIAApiException : Exception
    {
        internal NVIDIAApiException(Status status)
        {
            Status = status;
        }

        /// <inheritdoc />
        public override string Message
        {
            get => GeneralApi.GetErrorMessage(Status) ?? Status.ToString();
        }

        /// <summary>
        ///     Gets NVIDIA Api exception status code
        /// </summary>
        public Status Status { get; }
    }
}