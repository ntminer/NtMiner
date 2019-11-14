using LiteDB;
using NTMiner.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTMiner.KernelOutputKeyword {
    public class KernelOutputKeywordSet : IKernelOutputKeywordSet {
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();
        private readonly string _connectionString;

        private readonly bool _isServer;
        public KernelOutputKeywordSet(string dbFileFullName, bool isServer) {
            if (string.IsNullOrEmpty(dbFileFullName)) {
                throw new ArgumentNullException(nameof(dbFileFullName));
            }
            _connectionString = $"filename={dbFileFullName};journal=false";
            _isServer = isServer;
            if (!isServer) {
                VirtualRoot.BuildCmdPath<LoadKernelOutputKeywordCommand>(action: message => {
                    DateTime localTimestamp = VirtualRoot.LocalKernelOutputKeywordSetTimestamp;
                    // 如果已知服务器端最新内核输出关键字时间戳不比本地已加载的最新内核输出关键字时间戳新就不用加载了
                    if (message.KnowKernelOutputKeywordTimestamp <= Timestamp.GetTimestamp(localTimestamp)) {
                        return;
                    }
                    OfficialServer.KernelOutputKeywordService.GetKernelOutputKeywords((response, e) => {
                        if (response.IsSuccess()) {
                            Guid[] toRemoves = _dicById.Where(a => a.Value.DataLevel == DataLevel.Global).Select(a => a.Key).ToArray();
                            foreach (var id in toRemoves) {
                                _dicById.Remove(id);
                            }
                            if (response.Data.Count != 0) {
                                foreach (var item in response.Data) {
                                    item.SetDataLevel(DataLevel.Global);
                                    _dicById.Add(item.Id, item);
                                }
                                if (response.Timestamp != Timestamp.GetTimestamp(localTimestamp)) {
                                    VirtualRoot.LocalKernelOutputKeywordSetTimestamp = Timestamp.FromTimestamp(response.Timestamp);
                                }
                                CacheServerKernelOutputKeywords(response.Data);
                                VirtualRoot.RaiseEvent(new KernelOutputKeywordLoadedEvent(response.Data));
                            }
                        }
                    });
                });
            }
            VirtualRoot.BuildCmdPath<AddOrUpdateKernelOutputKeywordCommand>(action: (message) => {
                InitOnece();
                if (isServer || !DevMode.IsDevMode) {
                    DataLevel dataLevel = isServer ? DataLevel.Global : DataLevel.Profile;
                    if (_dicById.TryGetValue(message.Input.GetId(), out KernelOutputKeywordData exist)) {
                        exist.Update(message.Input);
                        exist.SetDataLevel(dataLevel);
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<KernelOutputKeywordData>();
                            col.Update(exist);
                        }
                        if (!isServer) {
                            VirtualRoot.RaiseEvent(new UserKernelOutputKeywordUpdatedEvent(exist));
                        }
                    }
                    else {
                        KernelOutputKeywordData entity = new KernelOutputKeywordData().Update(message.Input);
                        entity.SetDataLevel(dataLevel);
                        _dicById.Add(entity.Id, entity);
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<KernelOutputKeywordData>();
                            col.Insert(entity);
                        }
                        if (!isServer) {
                            VirtualRoot.RaiseEvent(new UserKernelOutputKeywordAddedEvent(exist));
                        }
                    }
                }
                else if (DevMode.IsDevMode) {
                    OfficialServer.KernelOutputKeywordService.AddOrUpdateKernelOutputKeywordAsync(KernelOutputKeywordData.Create(message.Input), (response, e) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
                        }
                    });
                }
            });
            VirtualRoot.BuildCmdPath<RemoveKernelOutputKeywordCommand>(action: (message) => {
                InitOnece();
                if (isServer || !DevMode.IsDevMode) {
                    if (message == null || message.EntityId == Guid.Empty) {
                        return;
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelOutputKeywordData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<KernelOutputKeywordData>();
                        col.Delete(message.EntityId);
                    }
                    if (!isServer) {
                        VirtualRoot.RaiseEvent(new UserKernelOutputKeywordRemovedEvent(entity));
                    }
                }
                else if (DevMode.IsDevMode) {
                    OfficialServer.KernelOutputKeywordService.RemoveKernelOutputKeyword(message.EntityId, (response, e) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
                        }
                    });
                }
            });
        }

        private const string fileName = "ServerKernelOutputKeywords.json";
        private static readonly Func<string> GetFileId = () => {
            return $"$/cache/{fileName}";
        };
        private void CacheServerKernelOutputKeywords(List<KernelOutputKeywordData> data) {
            string json = VirtualRoot.JsonSerializer.Serialize(data);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json))) {
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    db.FileStorage.Upload(GetFileId(), fileName, ms);
                }
            }
        }

        private List<KernelOutputKeywordData> GetServerKernelOutputKeywordsFromCache() {
            try {
                using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                    if (db.FileStorage.Exists(GetFileId())) {
                        using (MemoryStream ms = new MemoryStream()) {
                            db.FileStorage.Download(GetFileId(), ms);
                            var json = Encoding.UTF8.GetString(ms.ToArray());
                            return VirtualRoot.JsonSerializer.Deserialize<List<KernelOutputKeywordData>>(json);
                        }
                    }
                    else {
                        return new List<KernelOutputKeywordData>();
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return new List<KernelOutputKeywordData>();
            }
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
                    if (!_isServer) {
                        foreach (var item in GetServerKernelOutputKeywordsFromCache()) {
                            if (!_dicById.ContainsKey(item.GetId())) {
                                item.SetDataLevel(DataLevel.Global);
                                _dicById.Add(item.GetId(), item);
                            }
                        }
                    }
                    if (_isServer || !DevMode.IsDevMode) {
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<KernelOutputKeywordData>();
                            foreach (var item in col.FindAll()) {
                                if (!_dicById.ContainsKey(item.GetId())) {
                                    item.SetDataLevel(DataLevel.Profile);
                                    _dicById.Add(item.GetId(), item);
                                }
                            }
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerable<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId) {
            InitOnece();
            return _dicById.Values.Where(a => a.KernelOutputId == kernelOutputId);
        }

        public bool Contains(Guid kernelOutputId, string keyword) {
            InitOnece();
            return _dicById.Values.Any(a => a.KernelOutputId == kernelOutputId && a.Keyword == keyword);
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
