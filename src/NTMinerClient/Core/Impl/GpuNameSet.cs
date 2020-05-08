using LiteDB;
using NTMiner.Core.Gpus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly List<GpuName> _gpuNames = new List<GpuName>();

        public GpuNameSet() {
            if (ClientAppType.IsMinerClient) {
                _gpuNames.AddRange(GetGpuNamesFromCache());
                VirtualRoot.AddEventPath<HasBoot2SecondEvent>("启动一段时间后从服务器加载显卡特征名集合", LogEnum.DevConsole, action: message => {
                    Init();
                }, this.GetType());
                VirtualRoot.AddEventPath<Per20MinuteEvent>("周期刷新显卡特征集合", LogEnum.DevConsole, action: message => {
                    Init();
                }, this.GetType());
            }
        }

        private DateTime _initedOn = DateTime.MinValue;
        private void Init() {
            if (!ClientAppType.IsMinerClient) {
                return;
            }
            if (_initedOn.AddSeconds(10) > DateTime.Now) {
                return;
            }
            _initedOn = DateTime.Now;
            RpcRoot.OfficialServer.GpuNameService.GetGpuNamesAsync((response, e) => {
                if (response.IsSuccess() && response.Data != null) {
                    _gpuNames.Clear();
                    _gpuNames.AddRange(response.Data);
                    CacheGpuNames(response.Data);
                }
                VirtualRoot.RaiseEvent(new GpuNameSetInitedEvent());
            });
        }

        private const string fileName = "GpuNames.json";
        private static readonly Func<string> GetFileId = () => {
            return $"$/cache/{fileName}";
        };
        private void CacheGpuNames(List<GpuName> data) {
            string json = VirtualRoot.JsonSerializer.Serialize(data);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json))) {
                using (LiteDatabase db = new LiteDatabase(HomePath.LocalDbFileFullName)) {
                    db.FileStorage.Upload(GetFileId(), fileName, ms);
                }
            }
        }

        private List<GpuName> GetGpuNamesFromCache() {
            try {
                using (LiteDatabase db = new LiteDatabase(HomePath.LocalDbFileFullName)) {
                    if (db.FileStorage.Exists(GetFileId())) {
                        using (MemoryStream ms = new MemoryStream()) {
                            db.FileStorage.Download(GetFileId(), ms);
                            var json = Encoding.UTF8.GetString(ms.ToArray());
                            return VirtualRoot.JsonSerializer.Deserialize<List<GpuName>>(json) ?? new List<GpuName>();
                        }
                    }
                    else {
                        return new List<GpuName>();
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return new List<GpuName>();
            }
        }

        public IEnumerable<IGpuName> AsEnumerable() {
            if (!ClientAppType.IsMinerClient) {
                return new List<GpuName>();
            }
            if (_gpuNames.Count == 0) {
                Init();
            }
            return _gpuNames;
        }
    }
}
