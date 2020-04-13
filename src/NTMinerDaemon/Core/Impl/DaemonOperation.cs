using NTMiner.Controllers;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.RemoteDesktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NTMiner.Core.Impl {
    public class DaemonOperation : IDaemonOperation {
        private static readonly string _minerClientControllerName = RpcRoot.GetControllerName<IMinerClientController>();

        public DaemonOperation() { }

        public ResponseBase EnableRemoteDesktop() {
            ResponseBase response;
            try {
                NTMinerRegistry.SetIsRdpEnabled(true);
                Firewall.AddRdpRule();
                if (IsNTMinerOpened()) {
                    RpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, _minerClientControllerName, nameof(IMinerClientController.RefreshIsRemoteDesktopEnabled), null, null, timeountMilliseconds: 3000);
                }
                response = ResponseBase.Ok("开启Windows远程桌面");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase BlockWAU() {
            if (TryGetMinerClientLocation(out string location)) {
                Windows.Cmd.RunClose(location, $"{NTKeyword.ActionCmdParameterName}{MinerClient.MinerClientActionType.BlockWAU.ToString()}");
            }
            ResponseBase response = ResponseBase.Ok("禁用windows系统更新");
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase AtikmdagPatcher() {
            if (TryGetMinerClientLocation(out string location)) {
                Windows.Cmd.RunClose(location, $"{NTKeyword.ActionCmdParameterName}{MinerClient.MinerClientActionType.AtikmdagPatcher.ToString()}");
            }
            ResponseBase response = ResponseBase.Ok("A卡驱动签名，如果本机不是A卡将被忽略");
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase SwitchRadeonGpu(bool on) {
            if (TryGetMinerClientLocation(out string location)) {
                MinerClientActionType actionType = MinerClientActionType.SwitchRadeonGpuOn;
                if (!on) {
                    actionType = MinerClientActionType.SwitchRadeonGpuOff;
                }
                Windows.Cmd.RunClose(location, $"{NTKeyword.ActionCmdParameterName}{actionType.ToString()}");
            }
            ResponseBase response = ResponseBase.Ok($"{(on ? "开启" : "关闭")}A卡计算模式，如果本机不是A卡将被忽略");
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase FixIp() {
            throw new NotImplementedException();
        }

        public void CloseDaemon() {
            // 延迟100毫秒再退出从而避免当前的CloseDaemon请求尚未收到响应
            100.MillisecondsDelay().ContinueWith(t => {
                VirtualRoot.Exit();
            });
        }

        #region GetGpuProfilesJson
        public string GetGpuProfilesJson() {
            try {
                return SpecialPath.ReadGpuProfilesJsonFile();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }
        #endregion

        #region SaveGpuProfilesJson
        public bool SaveGpuProfilesJson(string json) {
            bool isSuccess = false;
            string description = "超频";
            try {
                SpecialPath.SaveGpuProfilesJsonFile(json);
                if (IsNTMinerOpened()) {
                    RpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, _minerClientControllerName, nameof(IMinerClientController.OverClock), null, null);
                }
                else {
                    description = "超频，挖矿端未启动，下次启动时生效";
                }
                isSuccess = true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.OperationResultSet.Add(new OperationResultData {
                Timestamp = Timestamp.GetTimestamp(),
                StateCode = isSuccess ? 200 : 500,
                ReasonPhrase = isSuccess ? "Ok" : "Fail",
                Description = description
            });
            return isSuccess;
        }
        #endregion

        public bool SetAutoBootStart(bool autoBoot, bool autoStart) {
            bool isSuccess = false;
            try {
                MinerProfileUtil.SetAutoStart(autoBoot, autoStart);
                if (IsNTMinerOpened()) {
                    RpcRoot.FirePostAsync(NTKeyword.Localhost, NTKeyword.MinerClientPort, _minerClientControllerName, nameof(IMinerClientController.RefreshAutoBootStart), null, null, timeountMilliseconds: 3000);
                }
                isSuccess = true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            VirtualRoot.OperationResultSet.Add(new OperationResultData {
                Timestamp = Timestamp.GetTimestamp(),
                StateCode = isSuccess ? 200 : 500,
                ReasonPhrase = isSuccess ? "Ok" : "Fail",
                Description = "设置"
            });
            return isSuccess;
        }

        public ResponseBase RestartWindows() {
            ResponseBase response;
            try {
                Windows.Power.Restart(10);
                CloseNTMiner();
                response = ResponseBase.Ok("重启矿机");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase ShutdownWindows() {
            ResponseBase response;
            try {
                Windows.Power.Shutdown(10);
                CloseNTMiner();
                response = ResponseBase.Ok("关闭矿机");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public bool IsNTMinerOpened() {
            if (TryGetMinerClientLocation(out string location)) {
                string processName = Path.GetFileNameWithoutExtension(location);
                Process[] processes = Process.GetProcessesByName(processName);
                return processes.Length != 0;
            }
            return false;
        }

        private bool TryGetMinerClientLocation(out string location) {
            location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
            return !string.IsNullOrEmpty(location) && File.Exists(location);
        }

        public ResponseBase StartMine(WorkRequest request) {
            ResponseBase response;
            if (request == null) {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else {
                try {
                    if (request.WorkId != Guid.Empty) {
                        File.WriteAllText(SpecialPath.NTMinerLocalJsonFileFullName, request.LocalJson);
                        File.WriteAllText(SpecialPath.NTMinerServerJsonFileFullName, request.ServerJson);
                    }
                    string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
                    if (IsNTMinerOpened()) {
                        WorkRequest innerRequest = new WorkRequest {
                            WorkId = request.WorkId
                        };
                        response = RpcRoot.Post<ResponseBase>(NTKeyword.Localhost, NTKeyword.MinerClientPort, _minerClientControllerName, nameof(IMinerClientController.StartMine), innerRequest);
                        response.Description = "开始挖矿";
                    }
                    else {
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string arguments = NTKeyword.AutoStartCmdParameterName;
                            if (request.WorkId != Guid.Empty) {
                                arguments += " --work";
                            }
                            Windows.Cmd.RunClose(location, arguments);
                            response = ResponseBase.Ok("开始挖矿");
                        }
                        else {
                            response = ResponseBase.ServerError("开始挖矿，未找到挖矿端程序");
                        }
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError(e.Message);
                }
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase StopMine() {
            ResponseBase response;
            try {
                if (!IsNTMinerOpened()) {
                    response = ResponseBase.Ok();
                }
                else {
                    RpcRoot.Post<ResponseBase>(NTKeyword.Localhost, NTKeyword.MinerClientPort, _minerClientControllerName, nameof(IMinerClientController.StopMine), new SignRequest());
                    response = ResponseBase.Ok("停止挖矿");
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                response = ResponseBase.ServerError(e.Message);
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        private void CloseNTMiner() {
            bool isClosed = false;
            ResponseBase response = RpcRoot.Post<ResponseBase>(NTKeyword.Localhost, NTKeyword.MinerClientPort, _minerClientControllerName, nameof(IMinerClientController.CloseNTMiner), new SignRequest { });
            isClosed = response.IsSuccess();
            if (!isClosed) {
                try {
                    string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
                    if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                        string processName = Path.GetFileNameWithoutExtension(location);
                        Windows.TaskKill.Kill(processName);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            }
        }

        public ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request) {
            ResponseBase response;
            if (request == null || string.IsNullOrEmpty(request.NTMinerFileName)) {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else {
                Task.Factory.StartNew(() => {
                    try {
                        string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string arguments = NTKeyword.UpgradeCmdParameterName + request.NTMinerFileName;
                            Windows.Cmd.RunClose(location, arguments);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                });
                response = ResponseBase.Ok("升级挖矿端");
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase SetVirtualMemory(Dictionary<string, int> data) {
            ResponseBase response;
            if (data == null || data.Count == 0) {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else {
                try {
                    VirtualRoot.DriveSet.SetVirtualMemory(data);
                    response = ResponseBase.Ok("设置虚拟内存，需重启电脑，重启电脑后生效");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError(e.Message);
                }
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }

        public ResponseBase SetLocalIps(List<LocalIpInput> data) {
            ResponseBase response;
            if (data == null || data.Count == 0) {
                response = ResponseBase.InvalidInput("参数错误");
            }
            else {
                try {
                    foreach (var localIpInput in data) {
                        VirtualRoot.Execute(new SetLocalIpCommand(localIpInput, localIpInput.IsAutoDNSServer));
                    }
                    response = ResponseBase.Ok("设置矿机IP，可能会掉线，通常过一会儿能恢复。");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    response = ResponseBase.ServerError(e.Message);
                }
            }
            VirtualRoot.OperationResultSet.Add(response.ToOperationResult());
            return response;
        }
    }
}
