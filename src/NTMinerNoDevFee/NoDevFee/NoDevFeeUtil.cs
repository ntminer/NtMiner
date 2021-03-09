using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.NoDevFee {
    public static unsafe partial class NoDevFeeUtil {
        public enum Kernel {
            Claymore,
            PhoenixMiner,
            GMiner
        }

        public class KernelInfo {
            public KernelInfo() { }

            public string ProcessName { get; set; }
            public Kernel Kernel { get; set; }
            public string CommandLineKeyword { get; set; }
        }

        private static readonly List<Regex> walRegexList = new List<Regex> {
            VirtualRoot.GetRegex(@"-ewal\s+(\w+)"),
            VirtualRoot.GetRegex(@"-wal\s+(\w+)"),
            VirtualRoot.GetRegex(@"--user\s+(\w+)")
        };
        private static readonly List<Regex> workerRegexList = new List<Regex> {
            VirtualRoot.GetRegex(@"-eworker\s+(\S+)"),
            VirtualRoot.GetRegex(@"-worker\s+(\S+)"),
            VirtualRoot.GetRegex(@"--user\s+\w+\.(\S+)"),
            VirtualRoot.GetRegex(@"-ewal\s+\w+\.(\S+)"),
            VirtualRoot.GetRegex(@"-wal\s+\w+\.(\S+)"),
        };
        private static readonly Regex ethWalletRegex = VirtualRoot.GetRegex(@"^0x\w{40}$");
        private static readonly List<KernelInfo> _kernelInfoes = new List<KernelInfo> {
            new KernelInfo {
                ProcessName = "EthDcrMiner64.exe",
                Kernel = Kernel.Claymore,
                CommandLineKeyword = string.Empty
            },
            new KernelInfo {
                ProcessName = "PhoenixMiner.exe",
                Kernel = Kernel.PhoenixMiner,
                CommandLineKeyword = string.Empty
            },
            new KernelInfo {
                ProcessName = "miner.exe",
                Kernel = Kernel.GMiner,
                CommandLineKeyword = "GMiner"
            }
        };
        private static bool TryGetCommandLine(out Kernel kernel, out string minerName, out string userWallet) {
            minerName = string.Empty;
            userWallet = string.Empty;
            kernel = Kernel.Claymore;
            try {
                List<string> lines = new List<string>();
                bool flag = false;
                foreach (var kernelInfo in _kernelInfoes) {
                    lines = Windows.WMI.GetCommandLines(kernelInfo.ProcessName);
                    if (lines.Count != 0) {
                        if (string.IsNullOrEmpty(kernelInfo.CommandLineKeyword) || lines.Any(a => a.IndexOf(kernelInfo.CommandLineKeyword, StringComparison.OrdinalIgnoreCase) != -1)) {
                            kernel = kernelInfo.Kernel;
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag) {
                    return false;
                }
                string text = string.Join(" ", lines) + " ";
                foreach (var regex in walRegexList) {
                    var matches = regex.Matches(text);
                    if (matches.Count != 0) {
                        userWallet = matches[matches.Count - 1].Groups[1].Value;
                        break;
                    }
                }
                foreach (var regex in workerRegexList) {
                    var matches = regex.Matches(text);
                    if (matches.Count != 0) {
                        minerName = matches[matches.Count - 1].Groups[1].Value;
                        break;
                    }
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
            if (!TryGetCommandLine(out Kernel kernel, out string minerName, out string userWallet)) {
                Stop();
                return;
            }
            if (!_isStopping) {
                return;
            }
            _isStopping = false;
            Logger.InfoDebugLine($"用户矿机名 {minerName}, 用户钱包 {userWallet}");
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
                            kernel: kernel,
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

        private static bool TryGetPosition(Kernel kernel, string workerName, string ansiText, out int position) {
            position = 0;
            if (ansiText.Contains("eth_submitLogin")) {
                int baseIndex = 0;
                int workNameLen = 0;
                switch (kernel) {
                    case Kernel.Claymore:
                        baseIndex = 85;
                        workNameLen = "eth1.0".Length;
                        // 比isPhoenixMiner多一个空格
                        if (ansiText.Contains($": \"{workerName}\",")) {
                            workNameLen = workerName.Length;
                        }
                        break;
                    case Kernel.PhoenixMiner:
                        baseIndex = 114;
                        if (ansiText.Contains($":\"{workerName}\",")) {
                            workNameLen = workerName.Length;
                        }
                        else {
                            workNameLen = "eth1.0".Length;
                        }
                        break;
                    case Kernel.GMiner:
                        baseIndex = 102;
                        break;
                    default:
                        break;
                }
                position = baseIndex + workNameLen;
            }
            return position != 0;
        }

        private static void RunDiversion(
            ref IntPtr divertHandle,
            Kernel kernel,
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
                            if (TryGetPosition(kernel, workerName, ansiText, out var position)) {
                                string wallet = EthWalletSet.Instance.GetOneWallet();
                                if (!string.IsNullOrEmpty(wallet)) {
                                    byte[] byteWallet = Encoding.ASCII.GetBytes(wallet);
                                    string dwallet = Encoding.UTF8.GetString(packet, position, byteWallet.Length);
                                    if (!dwallet.StartsWith(userWallet) && ethWalletRegex.IsMatch(dwallet)) {
                                        string dstIp = ipv4Header->DstAddr.ToString();
                                        var dstPort = tcpHdr->DstPort;
                                        Buffer.BlockCopy(byteWallet, 0, packet, position, byteWallet.Length);
                                        Logger.InfoDebugLine($"::第 {++counter} 次");
                                        Logger.InfoDebugLine($":: {dwallet}");
                                        //Logger.InfoDebugLine($":: {wallet}");
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
