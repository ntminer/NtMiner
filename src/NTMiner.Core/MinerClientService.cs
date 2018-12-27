using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private MinerClientService() {
        }

        // 创建一个128位数随机数，base64编码后返回
        private static string CreateAesKey() {
            byte[] data = new byte[16];// 128位
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(data);
            }
            return Convert.ToBase64String(data);
        }

        private static string EncryptSerialize<T>(T obj, out string desKey) {
            string json = Global.JsonSerializer.Serialize(obj);
            string key = CreateAesKey();
            desKey = Security.RSAHelper.EncryptString(key, ClientId.PrivateKey);
            string result = Security.AESHelper.Encrypt(json, key);
            return result;
        }

        private IMinerClientService CreateService(string host) {
            return ChannelFactory.CreateChannel<IMinerClientService>(
                    ChannelFactory.BasicHttpBinding,
                    host, 3336);
        }

        public void StartMine(string host, string pubKey, Guid workId, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    string desKey;
                    string data = EncryptSerialize(new Dictionary<string, object> {
                            {"workId", workId },
                            {"time", DateTime.Now }
                        }, out desKey);
                    using (var service = CreateService(host)) {
                        service.StartMine(desKey, data);
                    }
                    callback?.Invoke(true);
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    callback?.Invoke(false);
                }
                catch (Exception e) {
                    Global.Logger.Error(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StopMine(string host, string pubKey, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    string desKey;
                    string data = EncryptSerialize(new Dictionary<string, object> {
                            {"time", DateTime.Now }
                        }, out desKey);
                    using (var service = CreateService(host)) {
                        service.StopMine(desKey, data);
                    }
                    callback?.Invoke(true);
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    callback?.Invoke(false);
                }
                catch (Exception e) {
                    Global.Logger.Error(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void SetMinerProfileProperty(string host, string pubKey, string propertyName, object value, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    string desKey;
                    string data = EncryptSerialize(new Dictionary<string, object> {
                                {"propertyName",  propertyName},
                                {"value",  value},
                                {"time", DateTime.Now }
                            }, out desKey);
                    using (var service = CreateService(host)) {
                        service.SetMinerProfileProperty(desKey, data);
                    }
                    callback?.Invoke(true);
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    callback?.Invoke(false);
                }
                catch (Exception e) {
                    Global.Logger.Error(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void Dispose() {
            // nothing need todo
        }
    }
}
