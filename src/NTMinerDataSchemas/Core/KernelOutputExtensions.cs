namespace NTMiner.Core {
    public static class KernelOutputExtensions {
        public static bool GetIsRejectOneGpuShare(this IKernelOutput kernelOutput) {
            if (kernelOutput == null) {
                return false;
            }
            return !string.IsNullOrEmpty(kernelOutput.RejectOneShare) || !string.IsNullOrEmpty(kernelOutput.GpuRejectShare);
        }

        public static bool GetIsGotOneIncorrectGpuShare(this IKernelOutput kernelOutput) {
            if (kernelOutput == null) {
                return false;
            }
            if (!string.IsNullOrEmpty(kernelOutput.GpuGotOneIncorrectShare) && kernelOutput.GpuGotOneIncorrectShare.Contains("?<gpu>")) {
                return true;
            }
            if (!string.IsNullOrEmpty(kernelOutput.GpuIncorrectShare) && kernelOutput.GpuIncorrectShare.Contains("?<gpu>")) {
                return true;
            }
            return false;
        }

        public static bool GetIsFoundOneGpuShare(this IKernelOutput kernelOutput) {
            if (kernelOutput == null) {
                return false;
            }
            if (!string.IsNullOrEmpty(kernelOutput.FoundOneShare) && kernelOutput.FoundOneShare.Contains("?<gpu>")) {
                return true;
            }
            if (!string.IsNullOrEmpty(kernelOutput.AcceptOneShare) && kernelOutput.AcceptOneShare.Contains("?<gpu>")) {
                return true;
            }
            if (!string.IsNullOrEmpty(kernelOutput.GpuAcceptShare) && kernelOutput.GpuAcceptShare.Contains("?<gpu>")) {
                return true;
            }
            return false;
        }
    }
}
