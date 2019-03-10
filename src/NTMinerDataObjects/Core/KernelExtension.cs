namespace NTMiner.Core {
    public static class KernelExtension {
        public static string GetFullName(this IKernel kernel) {
            if (kernel == null) {
                return string.Empty;
            }

            return $"{kernel.Code} {kernel.Version}";
        }
    }
}
