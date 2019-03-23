using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Repositories {
    public class CompositeRepository<T> : IRepository<T> where T : class, ILevelEntity<Guid> {
        private readonly IRepository<T> _globalRepository;
        private readonly IRepository<T> _profileRepository;
        public CompositeRepository(IRepository<T> globalRepository, IRepository<T> profileRepository) {
            _globalRepository = globalRepository;
            _profileRepository = profileRepository;
        }

        public IEnumerable<T> GetAll() {
            var globalResults = _globalRepository.GetAll();
            var enumerable = globalResults as T[] ?? globalResults.ToArray();
            foreach (var item in enumerable) {
                item.SetDataLevel(DataLevel.Global);
            }
            if (DevMode.IsDebugMode) {
                return enumerable;
            }
            var list = enumerable.ToList();
            foreach (var item in _profileRepository.GetAll()) {
                item.SetDataLevel(DataLevel.Profile);
                list.Add(item);
            }
            return list;
        }

        public void Add(T entity) {
            if (DevMode.IsDebugMode) {
                entity.SetDataLevel(DataLevel.Global);
                _globalRepository.Add(entity);
            }
            else {
                entity.SetDataLevel(DataLevel.Profile);
                _profileRepository.Add(entity);
            }
        }

        public T GetByKey(Guid id) {
            if (DevMode.IsDebugMode) {
                var entity = _globalRepository.GetByKey(id);
                entity?.SetDataLevel(DataLevel.Global);
                return entity;
            }
            T result = _globalRepository.GetByKey(id);
            if (result == null) {
                result = _profileRepository.GetByKey(id);
                result?.SetDataLevel(DataLevel.Profile);
            }
            else {
                result.SetDataLevel(DataLevel.Global);
            }
            return result;
        }

        public bool Exists(Guid key) {
            if (DevMode.IsDebugMode) {
                return _globalRepository.Exists(key);
            }
            bool result = _globalRepository.Exists(key);
            if (!result) {
                result = _profileRepository.Exists(key);
            }
            return result;
        }

        public void Remove(Guid id) {
            if (DevMode.IsDebugMode) {
                _globalRepository.Remove(id);
            }
            else {
                _profileRepository.Remove(id);
            }
        }

        public void Update(T entity) {
            if (DevMode.IsDebugMode) {
                entity.SetDataLevel(DataLevel.Global);
                _globalRepository.Update(entity);
            }
            else {
                if (_profileRepository.Exists(entity.GetId())) {
                    entity.SetDataLevel(DataLevel.Profile);
                    _profileRepository.Update(entity);
                }
            }
        }
    }
}