using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    internal class CoinSet : SetBase, ICoinSet {
        private readonly Dictionary<string, CoinData> _dicByCode = new Dictionary<string, CoinData>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, CoinData> _dicById = new Dictionary<Guid, CoinData>();

        private readonly IServerContext _context;
        public CoinSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddCoinCommand>("添加币种", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("coin code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (_dicByCode.ContainsKey(message.Input.Code)) {
                        throw new ValidationException("编码重复");
                    }
                    CoinData entity = new CoinData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    _dicByCode.Add(entity.Code, entity);
                    var repository = context.CreateServerRepository<CoinData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new CoinAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdateCoinCommand>("更新币种", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("coin code can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out CoinData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<CoinData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new CoinUpdatedEvent(message.MessageId, message.Input));
                }, location: this.GetType());
            context.AddCmdPath<RemoveCoinCommand>("移除币种", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    CoinData entity = _dicById[message.EntityId];
                    Guid[] toRemoves = context.PoolSet.AsEnumerable().Where(a => a.CoinId == entity.Id).Select(a => a.GetId()).ToArray();
                    foreach (var id in toRemoves) {
                        VirtualRoot.Execute(new RemovePoolCommand(id));
                    }
                    toRemoves = context.CoinKernelSet.AsEnumerable().Where(a => a.CoinId == entity.Id).Select(a => a.GetId()).ToArray();
                    foreach (var id in toRemoves) {
                        VirtualRoot.Execute(new RemoveCoinKernelCommand(id));
                    }
                    toRemoves = NTMinerContext.Instance.MinerProfile.GetWallets().Where(a => a.CoinId == entity.Id).Select(a => a.GetId()).ToArray();
                    foreach (var id in toRemoves) {
                        VirtualRoot.Execute(new RemoveWalletCommand(id));
                    }
                    toRemoves = context.CoinGroupSet.AsEnumerable().Where(a => a.CoinId == entity.Id).Select(a => a.GetId()).ToArray();
                    foreach (var id in toRemoves) {
                        VirtualRoot.Execute(new RemoveCoinGroupCommand(id));
                    }
                    _dicById.Remove(entity.Id);
                    if (_dicByCode.ContainsKey(entity.Code)) {
                        _dicByCode.Remove(entity.Code);
                    }
                    var repository = context.CreateServerRepository<CoinData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new CoinRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<CoinData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
                }
                if (!_dicByCode.ContainsKey(item.Code)) {
                    _dicByCode.Add(item.Code, item);
                }
            }
        }

        public bool Contains(string coinCode) {
            InitOnece();
            return _dicByCode.ContainsKey(coinCode);
        }

        public bool Contains(Guid coinId) {
            InitOnece();
            return _dicById.ContainsKey(coinId);
        }

        public bool TryGetCoin(string coinCode, out ICoin coin) {
            InitOnece();
            var r = _dicByCode.TryGetValue(coinCode, out CoinData c);
            coin = c;
            return r;
        }

        public bool TryGetCoin(Guid coinId, out ICoin coin) {
            InitOnece();
            var r = _dicById.TryGetValue(coinId, out CoinData c);
            coin = c;
            return r;
        }

        public IEnumerable<ICoin> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
