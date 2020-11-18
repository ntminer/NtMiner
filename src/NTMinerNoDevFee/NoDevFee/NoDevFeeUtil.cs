using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.NoDevFee {
    public static unsafe partial class NoDevFeeUtil {
        private static bool TryGetClaymoreCommandLine(out bool isPhoenixMiner, out string minerName, out string userWallet) {
            minerName = string.Empty;
            userWallet = string.Empty;
            isPhoenixMiner = false;
            try {
                var lines = Windows.WMI.GetCommandLines("EthDcrMiner64.exe");
                if (lines.Count == 0) {
                    lines = Windows.WMI.GetCommandLines("PhoenixMiner.exe");
                    if (lines.Count == 0) {
                        return false;
                    }
                    else {
                        isPhoenixMiner = true;
                    }
                }
                string text = string.Join(" ", lines) + " ";
                const string walletPattern = @"-ewal\s+(\w+)\s";
                string minerNamePattern = isPhoenixMiner ? @"-worker\s+(\w+)\s" : @"-eworker\s+(\w+)\s";
                Regex regex = VirtualRoot.GetRegex(walletPattern);
                var matches = regex.Matches(text);
                if (matches.Count != 0) {
                    userWallet = matches[matches.Count - 1].Groups[1].Value;
                }
                regex = VirtualRoot.GetRegex(minerNamePattern);
                matches = regex.Matches(text);
                if (matches.Count != 0) {
                    minerName = matches[matches.Count - 1].Groups[1].Value;
                }
                return !string.IsNullOrEmpty(minerName) && !string.IsNullOrEmpty(userWallet);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        private static volatile bool _isStopping = true;
        public static void StartAsync() {
            // Win7下WinDivert.sys文件签名问题
            if (VirtualRoot.IsLTWin10) {
                return;
            }
            if (!TryGetClaymoreCommandLine(out bool isPhoenixMiner, out string minerName, out string userWallet)) {
                Stop();
                return;
            }
            if (!_isStopping) {
                return;
            }
            _isStopping = false;
            WaitHandle.Set();
            WaitHandle = new AutoResetEvent(false);
            Task.Factory.StartNew(() => {
                WinDivertExtract.Extract();
                int counter = 0;
                // 表示是否成功运行了一次
                bool isRunOk = false;

                string filter = $"outbound && ip && ip.DstAddr != 127.0.0.1 && tcp && tcp.PayloadLength > 100";
                IntPtr divertHandle = SafeNativeMethods.WinDivertOpen(filter, WINDIVERT_LAYER.WINDIVERT_LAYER_NETWORK, 0, 0);
                if (divertHandle != IntPtr.Zero) {
                    Task.Factory.StartNew(() => {
                        Logger.InfoDebugLine($"反水启动");
                        WaitHandle.WaitOne();
                        if (divertHandle != IntPtr.Zero) {
                            SafeNativeMethods.WinDivertClose(divertHandle);
                            divertHandle = IntPtr.Zero;
                        }
                        Logger.InfoDebugLine($"反水停止");
                    }, TaskCreationOptions.LongRunning);

                    int numberOfProcessors = Environment.ProcessorCount;
                    Logger.InfoDebugLine($"{numberOfProcessors}并行");
                    Parallel.ForEach(Enumerable.Range(0, numberOfProcessors), (Action<int>)(x => {
                        RunDiversion(
                            divertHandle: ref divertHandle,
                            isPhoenixMiner: isPhoenixMiner,
                            workerName: minerName,
                            userWallet: userWallet,
                            counter: ref counter,
                            isRunOk: ref isRunOk);
                    }));
                    Logger.OkDebugLine($"NoDevFee closed");
                }
                else {
                    Logger.WarnDebugLine($"NoDevFee start failed.");
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static void Stop() {
            _isStopping = true;
            WaitHandle.Set();
        }

        private static bool TryGetPosition(bool isPhoenixMiner, string workerName, string ansiText, out int position) {
            position = 0;
            if (ansiText.Contains("eth_submitLogin")) {
                int baseIndex;
                int workNameLen;
                if (isPhoenixMiner) {
                    baseIndex = 114;
                    if (ansiText.Contains($":\"{workerName}\",")) {
                        workNameLen = workerName.Length;
                    }
                    else if (ansiText.Contains("defaultRig0")) {
                        workNameLen = "defaultRig0".Length;
                    }
                    else {
                        workNameLen = "eth1.0".Length;
                    }
                }
                else {
                    baseIndex = 85;
                    workNameLen = "eth1.0".Length;
                    // 比isPhoenixMiner多一个空格
                    if (ansiText.Contains($": \"{workerName}\",")) {
                        workNameLen = workerName.Length;
                    }
                }
                position = baseIndex + workNameLen;
            }
            return position != 0;
        }

        private static void RunDiversion(
            ref IntPtr divertHandle,
            bool isPhoenixMiner,
            string workerName,
            string userWallet,
            ref int counter,
            ref bool isRunOk) {

            byte[] packet = new byte[65535];
            try {
                while (true) {
                    if (_isStopping) {
                        Logger.OkDebugLine("NoDevFee结束");
                        return;
                    }
                    uint readLength = 0;
                    WINDIVERT_IPHDR* ipv4Header = null;
                    WINDIVERT_TCPHDR* tcpHdr = null;
                    WINDIVERT_ADDRESS addr = new WINDIVERT_ADDRESS();

                    if (!SafeNativeMethods.WinDivertRecv(divertHandle, packet, (uint)packet.Length, ref addr, ref readLength)) {
                        Logger.InfoDebugLine("continue");
                        continue;
                    }

                    if (!isRunOk && readLength > 1) {
                        isRunOk = true;
                        Logger.InfoDebugLine("成功，运行中..");
                    }

                    fixed (byte* inBuf = packet) {
                        byte* payload = null;
                        SafeNativeMethods.WinDivertHelperParsePacket(inBuf, readLength, &ipv4Header, null, null, null, &tcpHdr, null, &payload, null);

                        if (ipv4Header != null && tcpHdr != null && payload != null) {
                            string ansiText = Marshal.PtrToStringAnsi((IntPtr)payload);
                            if (TryGetPosition(isPhoenixMiner, workerName, ansiText, out var position)) {
                                string wallet = EthWalletSet.Instance.GetOneWallet();
                                if (!string.IsNullOrEmpty(wallet)) {
                                    byte[] byteWallet = Encoding.ASCII.GetBytes(wallet);
                                    string dwallet = Encoding.UTF8.GetString(packet, position, byteWallet.Length);
                                    if (!dwallet.StartsWith(userWallet)) {
                                        string dstIp = ipv4Header->DstAddr.ToString();
                                        var dstPort = tcpHdr->DstPort;
                                        Buffer.BlockCopy(byteWallet, 0, packet, position, byteWallet.Length);
                                        Logger.InfoDebugLine($"::第 {++counter} 次");
                                        Logger.InfoDebugLine($":: {dwallet}");
                                        Logger.InfoDebugLine($":: {wallet}");
                                        Logger.InfoDebugLine($":: {dstIp}:{dstPort.ToString()}");
                                    }
                                }
                            }
                        }
                    }

                    SafeNativeMethods.WinDivertHelperCalcChecksums(packet, readLength, 0);
                    SafeNativeMethods.WinDivertSendEx(divertHandle, packet, readLength, 0, ref addr, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }
    }
}
