using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.NoDevFee {
    public static unsafe partial class NoDevFeeUtil {
        private static bool TryGetClaymoreCommandLine(out string minerName, out string userWallet) {
            minerName = string.Empty;
            userWallet = string.Empty;
            try {
                var lines = Windows.WMI.GetCommandLines("EthDcrMiner64.exe");
                if (lines.Count == 0) {
                    return false;
                }
                string text = string.Join(" ", lines) + " ";
                const string walletPattern = @"-ewal\s+(\w+)\s";
                const string minerNamePattern = @"-eworker\s+(\w+)\s";
                Regex regex = new Regex(walletPattern, RegexOptions.Compiled);
                var matches = regex.Matches(text);
                if (matches.Count != 0) {
                    userWallet = matches[matches.Count - 1].Groups[1].Value;
                }
                regex = new Regex(minerNamePattern, RegexOptions.Compiled);
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

        private static readonly string _defaultWallet = "0xEd44cF3679D627d3Cb57767EfAc1bdd9C9B8D143";
        private static string _wallet = _defaultWallet;
        public static void SetWallet(string wallet) {
            _wallet = wallet;
        }

        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        private static bool _isStopping = true;
        public static void StartAsync() {
            if (string.IsNullOrEmpty(_wallet) || _wallet.Length != _defaultWallet.Length) {
                _wallet = _defaultWallet;
            }
            if (!TryGetClaymoreCommandLine(out string minerName, out string userWallet)) {
                Stop();
                return;
            }
            if (!_isStopping) {
                return;
            }
            WaitHandle.Set();
            WaitHandle = new AutoResetEvent(false);
            Task.Factory.StartNew(() => {
                WinDivertExtract.Extract();
                int counter = 0;
                bool ranOnce = false;

                string filter = $"outbound && ip && ip.DstAddr != 127.0.0.1 && tcp && tcp.PayloadLength > 100";
                IntPtr divertHandle = WinDivertMethods.WinDivertOpen(filter, WINDIVERT_LAYER.WINDIVERT_LAYER_NETWORK, 0, 0);

                if (divertHandle != IntPtr.Zero) {
                    Task.Factory.StartNew(() => {
                        Logger.InfoDebugLine($"反水启动");
                        WaitHandle.WaitOne();
                        if (divertHandle != IntPtr.Zero) {
                            WinDivertMethods.WinDivertClose(divertHandle);
                            divertHandle = IntPtr.Zero;
                        }
                        Logger.InfoDebugLine($"反水停止");
                    }, TaskCreationOptions.LongRunning);

                    Logger.InfoDebugLine($"{Environment.ProcessorCount}并行");
                    _isStopping = false;
                    Parallel.ForEach(Enumerable.Range(0, Environment.ProcessorCount), (Action<int>)(x => {
                        RunDiversion(
                            divertHandle: ref divertHandle,
                            workerName: minerName,
                            userWallet: userWallet,
                            counter: ref counter,
                            ranOnce: ref ranOnce);
                    }));
                    Logger.OkDebugLine($"NoDevFee closed");
                }
                else {
                    Logger.WarnDebugLine($"NoDevFee start failed");
                }
            });
        }

        private static void Stop() {
            _isStopping = true;
            WaitHandle.Set();
        }

        private static bool TryGetPosition(string workerName, string ansiText, out int position) {
            position = 0;
            if (ansiText.Contains("eth_submitLogin")) {
                if (ansiText.Contains($": \"{workerName}\",")) {
                    position = 91 + workerName.Length - "eth1.0".Length;
                }
                else {
                    position = 91;
                }
            }
            return position != 0;
        }

        private static void RunDiversion(
            ref IntPtr divertHandle,
            string workerName,
            string userWallet,
            ref int counter,
            ref bool ranOnce) {

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

                    if (!WinDivertMethods.WinDivertRecv(divertHandle, packet, (uint)packet.Length, ref addr, ref readLength)) {
                        continue;
                    }

                    if (!ranOnce && readLength > 1) {
                        ranOnce = true;
                        Logger.InfoDebugLine("Diversion running..");
                    }

                    fixed (byte* inBuf = packet) {
                        byte* payload = null;
                        WinDivertMethods.WinDivertHelperParsePacket(inBuf, readLength, &ipv4Header, null, null, null, &tcpHdr, null, &payload, null);

                        if (ipv4Header != null && tcpHdr != null && payload != null) {
                            string text = Marshal.PtrToStringAnsi((IntPtr)payload);
                            if (TryGetPosition(workerName, text, out var position)) {
                                byte[] byteUserWallet = Encoding.ASCII.GetBytes(userWallet);
                                byte[] byteWallet = Encoding.ASCII.GetBytes(_wallet);
                                string dwallet = Encoding.UTF8.GetString(packet, position, byteWallet.Length);
                                if (!dwallet.StartsWith(userWallet)) {
                                    string dstIp = ipv4Header->DstAddr.ToString();
                                    var dstPort = tcpHdr->DstPort;
                                    Buffer.BlockCopy(byteWallet, 0, packet, position, byteWallet.Length);
                                    Logger.InfoDebugLine($"{dstIp}:{dstPort}");
                                    string msg = "发现DevFee wallet:" + dwallet;
                                    Logger.WarnDebugLine(msg);
                                    Logger.InfoDebugLine($"::Diverting DevFee {++counter}: ({DateTime.Now})");
                                    Logger.InfoDebugLine($"::Destined for: {dwallet}");
                                    Logger.InfoDebugLine($"::Diverted to :  {_wallet}");
                                    Logger.InfoDebugLine($"::Pool: {dstIp}:{dstPort}");
                                }
                            }
                        }
                    }

                    WinDivertMethods.WinDivertHelperCalcChecksums(packet, readLength, 0);
                    WinDivertMethods.WinDivertSendEx(divertHandle, packet, readLength, 0, ref addr, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }
    }
}
