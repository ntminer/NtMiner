using LiteDB;
using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTMiner.Impl {
    public class KernelOutputKeywordSet : SetBase, IKernelOutputKeywordSet {
        private readonly Dictionary<Guid, KernelOutputKeywordData> _dicById = new Dictionary<Guid, KernelOutputKeywordData>();
        private readonly Dictionary<Guid, List<IKernelOutputKeyword>> _dicByKernelOutputId = new Dictionary<Guid, List<IKernelOutputKeyword>>();
        private readonly string _connectionString;

        private readonly bool _isServer;
        public KernelOutputKeywordSet(string dbFileFullName, bool isServer) {
            if (string.IsNullOrEmpty(dbFileFullName)) {
                throw new ArgumentNullException(nameof(dbFileFullName));
            }
            _connectionString = $"filename={dbFileFullName}";
            _isServer = isServer;
            if (!isServer) {
                VirtualRoot.BuildCmdPath<LoadKernelOutputKeywordCommand>(path: message => {
                    DateTime localTimestamp = VirtualRoot.LocalKernelOutputKeywordSetTimestamp;
                    // 如果已知服务器端最新内核输出关键字时间戳不比本地已加载的最新内核输出关键字时间戳新就不用加载了
                    if (message.KnowKernelOutputKeywordTimestamp <= Timestamp.GetTimestamp(localTimestamp)) {
                        return;
                    }
                    RpcRoot.OfficialServer.KernelOutputKeywordService.GetKernelOutputKeywords((response, e) => {
                        if (response.IsSuccess()) {
                            KernelOutputKeywordData[] toRemoves = _dicById.Where(a => a.Value.GetDataLevel() == DataLevel.Global).Select(a => a.Value).ToArray();
                            foreach (var item in toRemoves) {
                                _dicById.Remove(item.Id);
                                if (_dicByKernelOutputId.TryGetValue(item.KernelOutputId, out List<IKernelOutputKeyword> list)) {
                                    list.Remove(item);
                                }
                            }
                            if (response.Data.Count != 0) {
                                foreach (var item in response.Data) {
                                    item.SetDataLevel(DataLevel.Global);
                                    _dicById.Add(item.Id, item);
                                    if (!_dicByKernelOutputId.TryGetValue(item.KernelOutputId, out List<IKernelOutputKeyword> list)) {
                                        list = new List<IKernelOutputKeyword>();
                                        _dicByKernelOutputId.Add(item.KernelOutputId, list);
                                    }
                                    list.Add(item);
                                }
                                if (response.Timestamp != Timestamp.GetTimestamp(localTimestamp)) {
                                    VirtualRoot.LocalKernelOutputKeywordSetTimestamp = Timestamp.FromTimestamp(response.Timestamp);
                                }
                                CacheServerKernelOutputKeywords(response.Data);
                                VirtualRoot.RaiseEvent(new KernelOutputKeywordLoadedEvent(response.Data));
                            }
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }, location: this.GetType());
            }
            VirtualRoot.BuildCmdPath<AddOrUpdateKernelOutputKeywordCommand>(path: (message) => {
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
                            VirtualRoot.RaiseEvent(new UserKernelOutputKeywordUpdatedEvent(message.MessageId, exist));
                        }
                    }
                    else {
                        KernelOutputKeywordData entity = new KernelOutputKeywordData().Update(message.Input);
                        entity.SetDataLevel(dataLevel);
                        _dicById.Add(entity.Id, entity);
                        if (!_dicByKernelOutputId.TryGetValue(entity.KernelOutputId, out List<IKernelOutputKeyword> list)) {
                            list = new List<IKernelOutputKeyword>();
                            _dicByKernelOutputId.Add(entity.KernelOutputId, list);
                        }
                        list.Add(entity);
                        using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                            var col = db.GetCollection<KernelOutputKeywordData>();
                            col.Insert(entity);
                        }
                        if (!isServer) {
                            VirtualRoot.RaiseEvent(new UserKernelOutputKeywordAddedEvent(message.MessageId, entity));
                        }
                    }
                }
                else if (DevMode.IsDevMode) {
                    message.Input.SetDataLevel(DataLevel.Global);
                    RpcRoot.OfficialServer.KernelOutputKeywordService.AddOrUpdateKernelOutputKeywordAsync(KernelOutputKeywordData.Create(message.Input), (response, e) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }
            }, location: this.GetType());
            VirtualRoot.BuildCmdPath<RemoveKernelOutputKeywordCommand>(path: (message) => {
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
                    if (_dicByKernelOutputId.TryGetValue(entity.KernelOutputId, out List<IKernelOutputKeyword> list)) {
                        list.Remove(entity);
                    }
                    using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                        var col = db.GetCollection<KernelOutputKeywordData>();
                        col.Delete(message.EntityId);
                    }
                    if (!isServer) {
                        VirtualRoot.RaiseEvent(new UserKernelOutputKeywordRemovedEvent(message.MessageId, entity));
                    }
                }
                else if (DevMode.IsDevMode) {
                    RpcRoot.OfficialServer.KernelOutputKeywordService.RemoveKernelOutputKeyword(message.EntityId, (response, e) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Execute(new LoadKernelOutputKeywordCommand());
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }
            }, location: this.GetType());
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
                            return VirtualRoot.JsonSerializer.Deserialize<List<KernelOutputKeywordData>>(json) ?? new List<KernelOutputKeywordData>();
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

        protected override void Init() {
            if (!_isServer) {
                foreach (var item in GetServerKernelOutputKeywordsFromCache()) {
                    if (!_dicById.ContainsKey(item.GetId())) {
                        item.SetDataLevel(DataLevel.Global);
                        _dicById.Add(item.GetId(), item);
                        if (!_dicByKernelOutputId.TryGetValue(item.KernelOutputId, out List<IKernelOutputKeyword> list)) {
                            list = new List<IKernelOutputKeyword>();
                            _dicByKernelOutputId.Add(item.KernelOutputId, list);
                        }
                        list.Add(item);
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
                            if (!_dicByKernelOutputId.TryGetValue(item.KernelOutputId, out List<IKernelOutputKeyword> list)) {
                                list = new List<IKernelOutputKeyword>();
                                _dicByKernelOutputId.Add(item.KernelOutputId, list);
                            }
                            list.Add(item);
                        }
                    }
                }
            }
        }

        public List<IKernelOutputKeyword> GetKeywords(Guid kernelOutputId) {
            InitOnece();
            if (_dicByKernelOutputId.TryGetValue(kernelOutputId, out List<IKernelOutputKeyword> list)) {
                return list;
            }
            return new List<IKernelOutputKeyword>();
        }

        public IEnumerable<IKernelOutputKeyword> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
