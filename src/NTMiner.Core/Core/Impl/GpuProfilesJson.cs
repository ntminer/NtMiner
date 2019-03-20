using NTMiner.Profile;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuProfilesJson {
        public GpuProfilesJson() {
            this.GpuProfiles = new List<GpuProfileData>();
        }

        private readonly object _locker = new object();
        private bool _inited = false;
        public void Init() {
            if (!_inited) {
                lock (_locker) {
                    if (!_inited) {
                        string json = SpecialPath.ReadGpuProfilesJsonFile();
                        if (!string.IsNullOrEmpty(json)) {
                            try {
                                GpuProfilesJson data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJson>(json);
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                        else {
                            GpuProfiles = new List<GpuProfileData>();
                        }
                        _inited = true;
                    }
                }
            }
        }

        public List<GpuProfileData> GpuProfiles { get; set; }
    }
}
