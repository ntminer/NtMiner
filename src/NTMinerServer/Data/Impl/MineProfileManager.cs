using LiteDB;
using NTMiner.ServiceContracts.DataObjects;
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
        private static Dictionary<string, PropertyInfo> _minerProfileProperties;
        private static Dictionary<string, PropertyInfo> MinerProfileProperties {
            get {
                if (_minerProfileProperties == null) {
                    _minerProfileProperties = new Dictionary<string, PropertyInfo>();
                    foreach (var item in typeof(MinerProfileData).GetProperties()) {
                        _minerProfileProperties.Add(item.Name, item);
                    }
                }
                return _minerProfileProperties;
            }
        }

        private static Dictionary<string, PropertyInfo> _coinProfileProperties;
        private static Dictionary<string, PropertyInfo> CoinProfileProperties {
            get {
                if (_coinProfileProperties == null) {
                    _coinProfileProperties = new Dictionary<string, PropertyInfo>();
                    foreach (var item in typeof(CoinProfileData).GetProperties()) {
                        _coinProfileProperties.Add(item.Name, item);
                    }
                }
                return _coinProfileProperties;
            }
        }

        private static Dictionary<string, PropertyInfo> _coinKernelProfileProperties;
        private static Dictionary<string, PropertyInfo> CoinKernelProfileProperties {
            get {
                if (_coinKernelProfileProperties == null) {
                    _coinKernelProfileProperties = new Dictionary<string, PropertyInfo>();
                    foreach (var item in typeof(CoinKernelProfileData).GetProperties()) {
                        _coinKernelProfileProperties.Add(item.Name, item);
                    }
                }
                return _coinKernelProfileProperties;
            }
        }
        #endregion

        public MinerProfileData GetMinerProfile(Guid workId) {
            using (var database = CreateDatabase(workId)) {
                var col = database.GetCollection<MinerProfileData>();
                var data = col.FindAll().FirstOrDefault();
                if (data == null) {
                    data = MinerProfileData.CreateDefaultData();
                    col.Insert(data);
                }
                return data;
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
                    data = MinerProfileData.CreateDefaultData();
                }
                PropertyInfo propertyInfo = MinerProfileProperties[propertyName];
                if (propertyInfo.PropertyType == typeof(Guid)) {
                    value = DictionaryExtensions.ConvertToGuid(value);
                }
                propertyInfo.SetValue(data, value, null);
                if (exist) {
                    data.ModifiedOn = DateTime.Now;
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
                    data.ModifiedOn = DateTime.Now;
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
                    data.ModifiedOn = DateTime.Now;
                    col.Update(data);
                }
                else {
                    col.Insert(data);
                }
            }
        }
    }
}
