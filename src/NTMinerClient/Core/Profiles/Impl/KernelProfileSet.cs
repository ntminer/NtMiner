using NTMiner.Core.Kernels;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.Core.Profiles.Impl {
    public class KernelProfileSet : IKernelProfileSet {
        private readonly Dictionary<Guid, KernelProfile> _dicByKernelId = new Dictionary<Guid, KernelProfile>();

        private readonly object _locker = new object();
        public KernelProfileSet() {
        }

        public IKernelProfile EmptyKernelProfile {
            get {
                return KernelProfile.Empty;
            }
        }

        public IKernelProfile GetKernelProfile(Guid kernelId) {
            if (_dicByKernelId.ContainsKey(kernelId)) {
                return _dicByKernelId[kernelId];
            }
            lock (_locker) {
                if (_dicByKernelId.ContainsKey(kernelId)) {
                    return _dicByKernelId[kernelId];
                }
                var kernelProfile = new KernelProfile() {
                    KernelId = kernelId
                };
                _dicByKernelId.Add(kernelId, kernelProfile);
                return kernelProfile;
            }
        }

        private class KernelProfile : IKernelProfile {
            public static readonly KernelProfile Empty = new KernelProfile();

            public KernelProfile() { }

            public Guid KernelId { get; set; }

            private IKernel _kernel;
            public IKernel Kernel {
                get {
                    if (_kernel == null) {
                        NTMinerContext.Instance.ServerContext.KernelSet.TryGetKernel(this.KernelId, out _kernel);
                    }
                    return _kernel;
                }
            }

            public InstallStatus InstallStatus {
                get {
                    if (this.KernelId == Guid.Empty || this.Kernel == null) {
                        return InstallStatus.Installed;
                    }
                    if (string.IsNullOrEmpty(this.Kernel.Package)) {
                        return InstallStatus.Uninstalled;
                    }
                    string packageFullName = this.Kernel.GetPackageFileFullName();
                    if (string.IsNullOrEmpty(packageFullName)) {
                        return InstallStatus.Uninstalled;
                    }
                    if (File.Exists(packageFullName)) {
                        return InstallStatus.Installed;
                    }
                    return InstallStatus.Uninstalled;
                }
            }
        }
    }
}
