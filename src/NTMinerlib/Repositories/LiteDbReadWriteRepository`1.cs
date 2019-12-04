using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Repositories {
    public class LiteDbReadWriteRepository<T> : IRepository<T> where T : class, IDbEntity<Guid> {
        private readonly string _connectionString;
        public LiteDbReadWriteRepository(string dbFile) {
            _connectionString = $"filename={dbFile};journal=false";
        }

        public IEnumerable<T> GetAll() {
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                return db.GetCollection<T>().FindAll().ToList();
            }
        }

        public void Add(T entity) {
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                var col = db.GetCollection<T>();
                T data = col.FindById(entity.GetId());
                if (data == null) {
                    col.Insert(entity);
                }
                else {
                    throw new DuplicateIdException();
                }
            }
        }

        public T GetByKey(Guid id) {
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                return db.GetCollection<T>().FindById(id);
            }
        }

        public bool Exists(Guid key) {
            return GetByKey(key) != null;
        }

        public void Remove(Guid id) {
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                db.GetCollection<T>().Delete(id);
            }
        }

        public void Update(T entity) {
            using (LiteDatabase db = new LiteDatabase(_connectionString)) {
                var col = db.GetCollection<T>();
                T data = col.FindById(entity.GetId());
                if (data != null) {
                    data.Update(entity);
                    col.Update(data);
                }
                else {
                    col.Insert(entity);
                }
            }
        }
    }
}
