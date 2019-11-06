using NTMiner.KernelOutputKeyword;
using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerServer.Impl {
    public class ServerKernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();

        private readonly INTMinerRoot _root;
        public ServerKernelOutputKeywordSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.BuildCmdPath<SetKernelOutputKeywordCommand>(action: message => {
                if (message.Input == null) {
                    return;
                }
                KernelOutputKeywordData oldValue;
                if (_dicById.TryGetValue(message.Input.GetId(), out KernelOutputKeywordData entity)) {
                    oldValue = KernelOutputKeywordData.Create(entity);
                    entity.Keyword = message.Input.Keyword;
                    entity.MessageType = message.Input.MessageType;
                }
                else {
                    entity = KernelOutputKeywordData.Create(message.Input);
                    oldValue = null;
                    _dicById.Add(message.Input.GetId(), entity);
                }
                OfficialServer.KernelOutputKeywordService.SetKernelOutputKeywordAsync(entity, (response, exception) => {
                    if (!response.IsSuccess()) {
                        if (oldValue == null) {
                            _dicById.Remove(message.Input.GetId());
                        }
                        else {
                            entity.Keyword = oldValue.Keyword;
                            entity.MessageType = oldValue.MessageType;
                        }
                        Write.UserFail(response.ReadMessage(exception));
                        VirtualRoot.RaiseEvent(new KernelOutputKeyworSetedEvent(entity));
                    }
                });
                VirtualRoot.RaiseEvent(new KernelOutputKeyworSetedEvent(entity));
            });
            VirtualRoot.BuildCmdPath<RemoveKernelOutputKeywordCommand>(action: message => {
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                KernelOutputKeywordData entity = _dicById[message.EntityId];
                OfficialServer.KernelOutputKeywordService.RemoveKernelOutputKeyword(entity.Id, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.RaiseEvent(new KernelOutputKeywordRemovedEvent(entity));
                    }
                    else {
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
            });
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
            VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
        }

        private bool _isInited = false;

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            if (_isInited) {
                return;
            }
            _isInited = true;
            OfficialServer.KernelOutputKeywordService.GetKernelOutputKeywords((response, e) => {
                if (response.IsSuccess()) {
                    foreach (var item in response.Data) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            item.SetDataLevel(DataLevel.Global);
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                }
                VirtualRoot.RaiseEvent(new KernelOutputKeywordSetInitedEvent());
            });
        }

        public bool Contains(Guid kernelOutputId, string keyword) {
            InitOnece();
            return _dicById.Values.Any(a => a.KernelOutputId == kernelOutputId && a.Keyword == keyword);
        }

        public IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId) {
            InitOnece();
            return _dicById.Values.Where(a => a.KernelOutputId == kernelOutputId);
        }

        public bool TryGetKernelOutputKeyword(Guid id, out IKernelOutputKeyword keyword) {
            InitOnece();
            var result = _dicById.TryGetValue(id, out KernelOutputKeywordData data);
            keyword = data;
            return result;
        }

        public IEnumerator<IKernelOutputKeyword> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
