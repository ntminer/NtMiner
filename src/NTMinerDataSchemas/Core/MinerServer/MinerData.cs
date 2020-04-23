using LiteDB;
using System;

namespace NTMiner.Core.MinerServer {
    public class MinerData : IMinerData {
        public static MinerData Create(string minerIp) {
            return new MinerData {
                Id = ObjectId.NewObjectId().ToString(),
                ClientId = Guid.NewGuid(),
                WorkerName = string.Empty,
                MinerName = string.Empty,
                CreatedOn = DateTime.Now,
                GroupId = Guid.Empty,
                LocalIp = minerIp,
                MinerIp = minerIp,
                MACAddress = string.Empty,
                WindowsLoginName = string.Empty,
                WindowsPassword = string.Empty,
                WorkId = Guid.Empty,
                IsOuterUserEnabled = false,
                LoginName = string.Empty,
                OuterUserId = string.Empty,
                AESPassword = string.Empty,
                AESPasswordOn = DateTime.MinValue
            };
        }

        public static MinerData Create(IMinerSign minerSign) {
            return new MinerData {
                Id = minerSign.Id,
                ClientId = minerSign.ClientId,
                LoginName = minerSign.LoginName,
                OuterUserId = minerSign.OuterUserId,
                AESPassword = minerSign.AESPassword,
                AESPasswordOn = minerSign.AESPasswordOn,
                WorkerName = string.Empty,
                MinerName = string.Empty,
                CreatedOn = DateTime.Now,
                GroupId = Guid.Empty,
                LocalIp = string.Empty,
                MinerIp = string.Empty,
                MACAddress = string.Empty,
                WindowsLoginName = string.Empty,
                WindowsPassword = string.Empty,
                WorkId = Guid.Empty,
                IsOuterUserEnabled = false
            };
        }

        public static MinerData Create(ClientData clientData) {
            return new MinerData {
                CreatedOn = clientData.CreatedOn,
                GroupId = clientData.GroupId,
                Id = clientData.Id,
                ClientId = clientData.ClientId,
                WorkerName = clientData.WorkerName,
                LocalIp = clientData.LocalIp,
                MinerIp = clientData.MinerIp,
                MACAddress = clientData.MACAddress,
                MinerName = clientData.MinerName,
                WindowsLoginName = clientData.WindowsLoginName,
                WindowsPassword = clientData.WindowsPassword,
                WorkId = clientData.WorkId,
                IsOuterUserEnabled = clientData.IsOuterUserEnabled,
                LoginName = clientData.LoginName,
                OuterUserId = clientData.OuterUserId,
                AESPassword = clientData.AESPassword,
                AESPasswordOn = clientData.AESPasswordOn
            };
        }

        public MinerData() { }

        public string Id { get; set; }
        public Guid ClientId { get; set; }
        public string LoginName { get; set; }
        public string OuterUserId { get; set; }
        public bool IsOuterUserEnabled { get; set; }
        public string AESPassword { get; set; }
        public DateTime AESPasswordOn { get; set; }

        public string WorkerName { get; set; }
        public string MinerName { get; set; }
        public Guid WorkId { get; set; }
        public string LocalIp { get; set; }
        public string MinerIp { get; set; }
        public string MACAddress { get; set; }
        public string WindowsLoginName { get; set; }

        private string _windowsPassword;
        public string WindowsPassword {
            get {
                return _windowsPassword;
            }
            set {
                if (!Base64Util.IsBase64OrEmpty(value)) {
                    value = string.Empty;
                }
                _windowsPassword = value;
            }
        }

        public DateTime CreatedOn { get; set; }
        public Guid GroupId { get; set; }
    }
}
