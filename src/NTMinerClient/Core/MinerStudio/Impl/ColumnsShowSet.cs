using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.MinerStudio.Impl {
    public class ColumnsShowSet : IColumnsShowSet {
        private readonly Dictionary<Guid, ColumnsShowData> _dicById = new Dictionary<Guid, ColumnsShowData>();

        public ColumnsShowSet() {
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
                    var repository = VirtualRoot.CreateLocalRepository<ColumnsShowData>();
                    var columnsList = repository.GetAll();
                    foreach (var item in columnsList) {
                        _dicById.Add(item.Id, item);
                    }
                    if (!_dicById.ContainsKey(ColumnsShowData.PleaseSelect.Id)) {
                        _dicById.Add(ColumnsShowData.PleaseSelect.Id, ColumnsShowData.PleaseSelect);
                        repository.Add(ColumnsShowData.PleaseSelect);
                    }
                    _isInited = true;
                }
            }
        }

        public List<ColumnsShowData> GetAll() {
            InitOnece();
            return _dicById.Values.ToList();
        }

        public void AddOrUpdate(ColumnsShowData data) {
            InitOnece();
            lock (_locker) {
                var repository = VirtualRoot.CreateLocalRepository<ColumnsShowData>();
                if (_dicById.TryGetValue(data.Id, out ColumnsShowData entity)) {
                    entity.Update(data);
                    repository.Update(entity);
                }
                else {
                    _dicById.Add(data.Id, data);
                    repository.Add(data);
                }
            }
            VirtualRoot.RaiseEvent(new ColumnsShowAddedOrUpdatedEvent(Guid.Empty, data));
        }

        public void Remove(Guid id) {
            InitOnece();
            ColumnsShowData entity;
            lock (_locker) {
                if (_dicById.TryGetValue(id, out entity)) {
                    _dicById.Remove(id);
                    var repository = VirtualRoot.CreateLocalRepository<ColumnsShowData>();
                    repository.Remove(id);
                }
            }
            if (entity != null) {
                VirtualRoot.RaiseEvent(new ColumnsRemovedEvent(Guid.Empty, entity));
            }
        }
    }
}
