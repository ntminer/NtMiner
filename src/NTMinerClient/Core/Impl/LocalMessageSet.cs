using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class LocalMessageSet : SetBase, ILocalMessageSet {
        private readonly LinkedList<ILocalMessage> _records = new LinkedList<ILocalMessage>();
        private readonly List<Guid> _dbToRemoveIds = new List<Guid>();
        private readonly List<LocalMessageData> _dbToInserts = new List<LocalMessageData>();

        private string ConnString {
            get {
                return $"filename={HomePath.LocalDbFileFullName}";
            }
        }

        public ILocalMessageDtoSet LocalMessageDtoSet { get; private set; } = EmptyLocalMessageDtoSet.Instance;

        public LocalMessageSet() {
            if (ClientAppType.IsMinerClient) {
                LocalMessageDtoSet = new LocalMessageDtoSet();
            }
            VirtualRoot.BuildCmdPath<AddLocalMessageCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                var data = LocalMessageData.Create(message.Input);
                List<ILocalMessage> removeds = new List<ILocalMessage>();
                lock (_dbToInserts) {
                    _records.AddFirst(data);
                    _dbToInserts.Add(data);
                    while (_records.Count > NTKeyword.LocalMessageSetCapacity) {
                        var toRemove = _records.Last.Value;
                        removeds.Add(toRemove);
                        _records.RemoveLast();
                    }
                    _dbToRemoveIds.AddRange(removeds.Select(a => a.Id));
                }
                LocalMessageDtoSet.Add(data.ToDto());
                VirtualRoot.RaiseEvent(new LocalMessageAddedEvent(message.MessageId, data, removeds));
            });
            VirtualRoot.BuildCmdPath<ClearLocalMessageSetCommand>(location: this.GetType(), LogEnum.DevConsole, path: message => {
                lock (_dbToInserts) {
                    _records.Clear();
                    _dbToRemoveIds.Clear();
                    _dbToInserts.Clear();
                }
                try {
                    using (LiteDatabase db = new LiteDatabase(ConnString)) {
                        db.DropCollection(nameof(LocalMessageData));
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                VirtualRoot.RaiseEvent(new LocalMessageSetClearedEvent());
            });
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("周期保存LocalMessage到数据库", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                SaveToDb();
            });
            VirtualRoot.BuildEventPath<AppExitEvent>("程序退出时保存LocalMessage到数据库", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                SaveToDb();
            });
        }

        private void SaveToDb() {
            if (_dbToInserts.Count > 0) {
                lock (_dbToInserts) {
                    if (_dbToInserts.Count > 0) {
                        List<Guid> toRemoveIds = new List<Guid>();
                        List<LocalMessageData> toInserts = new List<LocalMessageData>();
                        toRemoveIds.AddRange(_dbToRemoveIds);
                        toInserts.AddRange(_dbToInserts);
                        _dbToRemoveIds.Clear();
                        _dbToInserts.Clear();
                        try {
                            using (LiteDatabase db = new LiteDatabase(ConnString)) {
                                var col = db.GetCollection<LocalMessageData>();
                                foreach (Guid id in toRemoveIds) {
                                    col.Delete(id);
                                }
                                if (toInserts.Count != 0) {
                                    col.InsertBulk(toInserts);
                                }
                            }
                        }
                        catch {
                        }
                    }
                }
            }
        }

        protected override void Init() {
            try {
                using (LiteDatabase db = new LiteDatabase(ConnString)) {
                    var col = db.GetCollection<LocalMessageData>();
                    if (col.Count() > 0) {
                        foreach (var item in col.FindAll().OrderBy(a => a.Timestamp)) {
                            if (_records.Count < NTKeyword.LocalMessageSetCapacity) {
                                _records.AddFirst(item);
                            }
                            else {
                                col.Delete(item.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                try {
                    using (LiteDatabase db = new LiteDatabase(ConnString)) {
                        db.DropCollection(nameof(LocalMessageData));
                    }
                }
                catch {
                }
            }
            foreach (var item in _records.Take(50).Reverse()) {
                LocalMessageDtoSet.Add(item.ToDto());
            }
        }

        public IEnumerable<ILocalMessage> AsEnumerable() {
            InitOnece();
            return _records;
        }
    }
}
