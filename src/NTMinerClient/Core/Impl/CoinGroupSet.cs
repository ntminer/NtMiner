using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CoinGroupSet : SetBase, ICoinGroupSet {
        private readonly Dictionary<Guid, CoinGroupData> _dicById = new Dictionary<Guid, CoinGroupData>();

        private readonly IServerContext _context;
        public CoinGroupSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddCoinGroupCommand>("添加币组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    CoinGroupData entity = new CoinGroupData().Update(message.Input);
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    _dicById.Add(entity.Id, entity);
                    var repository = context.CreateServerRepository<CoinGroupData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new CoinGroupAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveCoinGroupCommand>("移除币组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    CoinGroupData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = context.CreateServerRepository<CoinGroupData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new CoinGroupRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<CoinGroupData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
                }
            }
        }

        public List<Guid> GetGroupCoinIds(Guid groupId) {
            InitOnece();
            return _dicById.Values.Where(a => a.GroupId == groupId).Select(a => a.CoinId).ToList();
        }

        public IEnumerable<ICoinGroup> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
