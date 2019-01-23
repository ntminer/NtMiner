using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ProfileServiceFace {
            public static readonly ProfileServiceFace Instance = new ProfileServiceFace();

            private ProfileServiceFace() {
            }

            private IProfileService CreateService() {
                return ChannelFactory.CreateChannel<IProfileService>(Server.MinerServerHost, Server.MinerServerPort);
            }

            public void GetMineWorkAsync(Guid workId, Action<MineWorkData> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            callback?.Invoke(service.GetMineWork(workId));
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(null);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(null);
                    }
                });
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <returns></returns>
            public List<MineWorkData> GetMineWorks() {
                try {
                    using (var service = CreateService()) {
                        return service.GetMineWorks();
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return new List<MineWorkData>();
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return new List<MineWorkData>();
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <returns></returns>
            public MinerProfileData GetMinerProfile(Guid workId) {
                try {
                    using (var service = CreateService()) {
                        return service.GetMinerProfile(workId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return null;
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinId"></param>
            /// <returns></returns>
            public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
                try {
                    using (var service = CreateService()) {
                        return service.GetCoinProfile(workId, coinId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return null;
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="poolId"></param>
            /// <returns></returns>
            public PoolProfileData GetPoolProfile(Guid workId, Guid poolId) {
                try {
                    using (var service = CreateService()) {
                        return service.GetPoolProfile(workId, poolId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return null;
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="workId"></param>
            /// <param name="coinKernelId"></param>
            /// <returns></returns>
            public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
                try {
                    using (var service = CreateService()) {
                        return service.GetCoinKernelProfile(workId, coinKernelId);
                    }
                }
                catch (CommunicationException e) {
                    Global.DebugLine(e.Message, ConsoleColor.Red);
                    return null;
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    return null;
                }
            }
        }
    }
}