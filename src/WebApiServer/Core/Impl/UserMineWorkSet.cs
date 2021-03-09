using LiteDB;
using NTMiner.Core.Daemon;
using NTMiner.Core.Mq.Senders;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class UserMineWorkSet : SetBase, IUserMineWorkSet {
        private readonly Dictionary<Guid, UserMineWorkData> _dicById = new Dictionary<Guid, UserMineWorkData>();

        public UserMineWorkSet(IOperationMqSender mq) {
            VirtualRoot.BuildEventPath<StartMineMqEvent>("收到StartMineMq消息后检查是否是作业挖矿，如果是则处理，否则忽略", LogEnum.None, path: message => {
                #region
                if (IsTooOld(message.Timestamp)) {
                    return;
                }
                Guid workId = message.Data;
                // 只处理作业的
                if (workId != Guid.Empty) {
                    try {
                        string workerName = string.Empty;
                        WorkRequest request;
                        // 如果是单机作业
                        if (workId.IsSelfMineWorkId()) {
                            var clientData = AppRoot.ClientDataSet.GetByClientId(message.ClientId);
                            if (clientData != null) {
                                workerName = clientData.WorkerName;
                            }
                            request = new WorkRequest {
                                WorkId = workId,
                                WorkerName = workerName,
                                LocalJson = string.Empty,
                                ServerJson = string.Empty
                            };
                        }
                        else {
                            IUserMineWork mineWork = AppRoot.MineWorkSet.GetById(workId);
                            if (mineWork == null) {
                                return;
                            }
                            string localJsonFileFullName = SpecialPath.GetMineWorkLocalJsonFileFullName(workId);
                            string localJson = string.Empty;
                            if (File.Exists(localJsonFileFullName)) {
                                localJson = File.ReadAllText(localJsonFileFullName);
                                if (!string.IsNullOrEmpty(localJson)) {
                                    var clientData = AppRoot.ClientDataSet.GetByClientId(message.ClientId);
                                    if (clientData != null) {
                                        workerName = clientData.WorkerName;
                                    }
                                    localJson = localJson.Replace(NTKeyword.MinerNameParameterName, workerName);
                                }
                            }
                            string serverJsonFileFullName = SpecialPath.GetMineWorkServerJsonFileFullName(workId);
                            string serverJson = string.Empty;
                            if (File.Exists(serverJsonFileFullName)) {
                                serverJson = File.ReadAllText(serverJsonFileFullName);
                            }
                            request = new WorkRequest {
                                WorkId = workId,
                                WorkerName = workerName,
                                LocalJson = localJson,
                                ServerJson = serverJson
                            };
                        }
                        mq.SendStartWorkMine(message.LoginName, message.ClientId, request);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
                #endregion
            }, this.GetType());
        }

        private static bool IsTooOld(DateTime dateTime) {
            return dateTime.AddSeconds(30) < DateTime.Now;
        }

        protected override void Init() {
            using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                var col = db.GetCollection<UserMineWorkData>();
                foreach (var item in col.FindAll()) {
                    _dicById.Add(item.Id, item);
                }
            }
        }

        public UserMineWorkData GetById(Guid workId) {
            InitOnece();
            if (_dicById.ContainsKey(workId)) {
                return _dicById[workId];
            }
            return null;
        }

        public List<UserMineWorkData> GetsByLoginName(string loginName) {
            InitOnece();
            return _dicById.Values.Where(a => a.LoginName == loginName).ToList();
        }

        public void AddOrUpdate(UserMineWorkData data) {
            InitOnece();
            lock (_dicById) {
                using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                    var col = db.GetCollection<UserMineWorkData>();
                    if (_dicById.TryGetValue(data.Id, out UserMineWorkData entity)) {
                        data.ModifiedOn = DateTime.Now;
                        entity.Update(data);
                        col.Update(entity);
                    }
                    else {
                        data.CreatedOn = DateTime.Now;
                        _dicById.Add(data.Id, data);
                        col.Insert(data);
                    }
                }
            }
        }

        public void RemoveById(Guid id) {
            InitOnece();
            lock (_dicById) {
                if (_dicById.ContainsKey(id)) {
                    _dicById.Remove(id);
                    using (LiteDatabase db = AppRoot.CreateLocalDb()) {
                        var col = db.GetCollection<UserMineWorkData>();
                        col.Delete(id);
                    }
                    SpecialPath.DeleteMineWorkFiles(id);
                }
            }
        }
    }
}
