using NTMiner.KernelOutputKeyword;
using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class ServerKernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();

        private readonly INTMinerRoot _root;
        public ServerKernelOutputKeywordSet(INTMinerRoot root) {
            _root = root;
        }

        public DateTime GetServerChannelTimestamp() {
            string serverChannelTimestamp = string.Empty;
            if (_root.LocalAppSettingSet.TryGetAppSetting(NTKeyword.ServerChannelTimestampAppSettingKey, out IAppSetting setting) && setting.Value != null) {
                serverChannelTimestamp = setting.Value.ToString();
            }
            if (string.IsNullOrEmpty(serverChannelTimestamp)) {
                return DateTime.MinValue;
            }
            if (DateTime.TryParse(serverChannelTimestamp, out DateTime timestamp)) {
                return timestamp;
            }
            return DateTime.MinValue;
        }

        private void SetServerChannelTimeStamp(DateTime timestamp) {
            AppSettingData appSettingData = new AppSettingData() {
                Key = NTKeyword.ServerChannelTimestampAppSettingKey,
                Value = timestamp
            };
            VirtualRoot.Execute(new ChangeLocalAppSettingCommand(appSettingData));
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var list = Server.KernelOutputKeywordService.GetKernelOutputKeywords();
                    foreach (var item in list) {
                        _dicById.Add(item.Id, item);
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid kernelOutputId, string keyword) {
            throw new NotImplementedException();
        }

        public IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId) {
            throw new NotImplementedException();
        }

        public bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword) {
            throw new NotImplementedException();
        }

        public IEnumerator<IKernelOutputKeyword> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
