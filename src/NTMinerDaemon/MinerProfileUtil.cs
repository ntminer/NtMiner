using LiteDB;
using NTMiner.Core.Profile;
using System.IO;

namespace NTMiner {
    public static class MinerProfileUtil {
        #region IsAutoBoot
        public static bool GetIsAutoBoot() {
            var db = GetDb();
            if (db != null) {
                using (db) {
                    MinerProfileData data = db.GetCollection<MinerProfileData>().FindById(MinerProfileData.DefaultId);
                    if (data != null) {
                        return data.IsAutoBoot;
                    }
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public static void SetAutoStart(bool isAutoBoot, bool isAutoStart) {
            var db = GetDb();
            if (db != null) {
                using (db) {
                    var col = db.GetCollection<MinerProfileData>();
                    MinerProfileData data = col.FindById(MinerProfileData.DefaultId);
                    if (data != null) {
                        data.IsAutoBoot = isAutoBoot;
                        data.IsAutoStart = isAutoStart;
                        col.Update(data);
                    }
                }
            }
        }

        private static LiteDatabase GetDb() {
            string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
            if (!string.IsNullOrEmpty(location)) {
                string dbFile = Path.Combine(Path.GetDirectoryName(location), NTKeyword.LocalDbFileName);
                bool isDbFileExist = File.Exists(dbFile);
                if (!isDbFileExist) {
                    dbFile = Path.Combine(EntryAssemblyInfo.TempDirFullName, NTKeyword.LocalDbFileName);
                    isDbFileExist = File.Exists(dbFile);
                }
                if (!isDbFileExist) {
                    return null;
                }
                return new LiteDatabase($"filename={dbFile}");
            }
            else {
                return null;
            }
        }
        #endregion
    }
}
