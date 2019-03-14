using LiteDB;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Data.Impl {
    public class MineProfileManager : IMineProfileManager {
        private readonly IHostRoot _root;
        public MineProfileManager(IHostRoot root) {
            _root = root;
        }

        private LiteDatabase CreateDatabase(Guid workId) {
            return new LiteDatabase($"filename={SpecialPath.GetMineWorkDbFileFullName(workId)};journal=false");
        }

        #region 元数据
        private static Dictionary<string, PropertyInfo> GetProperties<T>() {
            var result = new Dictionary<string, PropertyInfo>();
            foreach (var propertyInfo in typeof(T).GetProperties()) {
                result.Add(propertyInfo.Name, propertyInfo);
            }
            return result;
        }

        private static Dictionary<string, PropertyInfo> s_minerProfileProperties;
        private static Dictionary<string, PropertyInfo> MinerProfileProperties {
            get {
                if (s_minerProfileProperties == null) {
                    s_minerProfileProperties = GetProperties<MinerProfileData>();
                }
                return s_minerProfileProperties;
            }
        }

        private static Dictionary<string, PropertyInfo> s_coinProfileProperties;
        private static Dictionary<string, PropertyInfo> CoinProfileProperties {
            get {
                if (s_coinProfileProperties == null) {
                    s_coinProfileProperties = GetProperties<CoinProfileData>();
                }
                return s_coinProfileProperties;
            }
        }

        private static Dictionary<string, PropertyInfo> s_poolProfileProperties;
        private static Dictionary<string, PropertyInfo> PoolProfileProperties {
            get {
                if (s_poolProfileProperties == null) {
                    s_poolProfileProperties = GetProperties<PoolProfileData>();
                }
                return s_poolProfileProperties;
            }
        }

        private static Dictionary<string, PropertyInfo> s_coinKernelProfileProperties;
        private static Dictionary<string, PropertyInfo> CoinKernelProfileProperties {
            get {
                if (s_coinKernelProfileProperties == null) {
                    s_coinKernelProfileProperties = GetProperties<CoinKernelProfileData>();
                }
                return s_coinKernelProfileProperties;
            }
        }
        #endregion

        public MinerProfileData GetMinerProfile(Guid workId) {
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<MinerProfileData>();
                var data = col.FindAll().FirstOrDefault();
                return data;
            }
        }

        public void SetMinerProfile(Guid workId, MinerProfileData data) {
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<MinerProfileData>();
                col.Delete(Query.All());
                col.Insert(data);
            }
        }

        public void SetMinerProfileProperty(Guid workId, string propertyName, object value) {
            if (!MinerProfileProperties.ContainsKey(propertyName)) {
                return;
            }
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<MinerProfileData>();
                var data = col.FindAll().FirstOrDefault();
                bool exist = true;
                if (data == null) {
                    exist = false;
                    data = MinerProfileData.CreateDefaultData(Guid.Empty);
                }
                PropertyInfo propertyInfo = MinerProfileProperties[propertyName];
                if (propertyInfo.PropertyType == typeof(Guid)) {
                    value = DictionaryExtensions.ConvertToGuid(value);
                }
                propertyInfo.SetValue(data, value, null);
                if (exist) {
                    col.Update(data);
                }
                else {
                    col.Insert(data);
                }
            }
        }

        public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<CoinProfileData>();
                var data = col.FindById(coinId);
                if (data == null) {
                    data = CoinProfileData.CreateDefaultData(coinId);
                    col.Insert(data);
                }
                return data;
            }
        }

        public void SetCoinProfileProperty(Guid workId, Guid coinId, string propertyName, object value) {
            if (!CoinProfileProperties.ContainsKey(propertyName)) {
                return;
            }
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<CoinProfileData>();
                var data = col.FindById(coinId);
                bool exist = true;
                if (data == null) {
                    exist = false;
                    data = CoinProfileData.CreateDefaultData(coinId);
                }
                PropertyInfo propertyInfo = CoinProfileProperties[propertyName];
                if (propertyInfo.PropertyType == typeof(Guid)) {
                    value = DictionaryExtensions.ConvertToGuid(value);
                }
                propertyInfo.SetValue(data, value, null);
                if (exist) {
                    col.Update(data);
                }
                else {
                    col.Insert(data);
                }
            }
        }

        public PoolProfileData GetPoolProfile(Guid workId, Guid poolId) {
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<PoolProfileData>();
                var data = col.FindById(poolId);
                if (data == null) {
                    data = PoolProfileData.CreateDefaultData(poolId);
                    col.Insert(data);
                }
                return data;
            }
        }

        public void SetPoolProfileProperty(Guid workId, Guid poolId, string propertyName, object value) {
            if (!PoolProfileProperties.ContainsKey(propertyName)) {
                return;
            }
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<PoolProfileData>();
                var data = col.FindById(poolId);
                bool exist = true;
                if (data == null) {
                    exist = false;
                    data = PoolProfileData.CreateDefaultData(poolId);
                }
                PropertyInfo propertyInfo = PoolProfileProperties[propertyName];
                if (propertyInfo.PropertyType == typeof(Guid)) {
                    value = DictionaryExtensions.ConvertToGuid(value);
                }
                propertyInfo.SetValue(data, value, null);
                if (exist) {
                    col.Update(data);
                }
                else {
                    col.Insert(data);
                }
            }
        }

        public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<CoinKernelProfileData>();
                var data = col.FindById(coinKernelId);
                if (data == null) {
                    data = CoinKernelProfileData.CreateDefaultData(coinKernelId);
                    col.Insert(data);
                }
                return data;
            }
        }

        public void SetCoinKernelProfileProperty(Guid workId, Guid coinKernelId, string propertyName, object value) {
            if (!CoinKernelProfileProperties.ContainsKey(propertyName)) {
                return;
            }
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<CoinKernelProfileData>();
                var data = col.FindById(coinKernelId);
                bool exist = true;
                if (data == null) {
                    exist = false;
                    data = CoinKernelProfileData.CreateDefaultData(coinKernelId);
                }
                PropertyInfo propertyInfo = CoinKernelProfileProperties[propertyName];
                if (propertyInfo.PropertyType == typeof(Guid)) {
                    value = DictionaryExtensions.ConvertToGuid(value);
                }
                propertyInfo.SetValue(data, value, null);
                if (exist) {
                    col.Update(data);
                }
                else {
                    col.Insert(data);
                }
            }
        }

        public GpuProfileData GetGpuProfile(Guid workId, Guid coinId) {
            throw new NotImplementedException();
        }

        public void SetGpuProfile(Guid workId, Guid coinId, string propertyName, object value) {
            throw new NotImplementedException();
        }
    }
}
