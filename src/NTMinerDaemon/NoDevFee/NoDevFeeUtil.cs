using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.NoDevFee {
    public unsafe static partial class NoDevFeeUtil {
        private static int s_contextId;
        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        public static void StartAsync(
            int contextId, 
            string minerName,
            string coin, 
            string ourWallet,
            string testWallet,
            string kernelFullName,
            out string message) {
            CoinKernelId coinKernelId = CoinKernelId.Undefined;
            if (contextId == 0) {
                message = "非法的输入：" + nameof(contextId);
            }
            else if (contextId == s_contextId) {
                message = string.Empty;
            }
            else if (string.IsNullOrEmpty(coin)) {
                message = "非法的输入：" + nameof(coin);
            }
            else if (!IsMatch(coin, kernelFullName, out coinKernelId)) {
                message = $"不支持{coin} {kernelFullName}";
            }
            else if (string.IsNullOrEmpty(ourWallet)) {
                message = "没有ourWallet";
            }
            else if (string.IsNullOrEmpty(testWallet)) {
                message = "没有testWallet";
            }
            else {
                message = "ok";
            }
            if (!string.IsNullOrEmpty(message)) {
                Logger.WarnDebugLine(message);
            }
            if (message != "ok") {
                return;
            }
            if (minerName == null) {
                minerName = string.Empty;
            }
            s_contextId = contextId;
            WaitHandle.Set();
            WaitHandle = new AutoResetEvent(false);
            Task.Factory.StartNew(() => {
                WinDivertExtract.Extract();
                int counter = 0;
                bool ranOnce = false;

                string filter = $"outbound && ip && ip.DstAddr != 127.0.0.1 && tcp && tcp.PayloadLength > 100";
                Logger.InfoDebugLine(filter);
                IntPtr divertHandle = WinDivertNativeMethods.WinDivertOpen(filter, WINDIVERT_LAYER.WINDIVERT_LAYER_NETWORK, 0, 0);
                
                if (divertHandle != IntPtr.Zero) {
                    Task.Factory.StartNew(() => {
                        Logger.InfoDebugLine($"{coin} divertHandle 守护程序开启");
                        WaitHandle.WaitOne();
                        if (divertHandle != IntPtr.Zero) {
                            WinDivertNativeMethods.WinDivertClose(divertHandle);
                            divertHandle = IntPtr.Zero;
                        }
                        Logger.InfoDebugLine($"{coin} divertHandle 守护程序结束");
                    });

                    Logger.InfoDebugLine($"{Environment.ProcessorCount}并行");
                    Parallel.ForEach(Enumerable.Range(0, Environment.ProcessorCount), (Action<int>)(x => {
                        RunDiversion(
                            divertHandle: ref divertHandle, 
                            contextId: contextId, 
                            workerName: minerName, 
                            coin: coin, 
                            ourWallet: ourWallet,
                            testWallet: testWallet,
                            kernelFullName: kernelFullName,
                            coinKernelId: coinKernelId,
                            counter: ref counter,
                            ranOnce: ref ranOnce);
                    }));
                    Logger.OkDebugLine($"{coin} NoDevFee closed");
                }
                else {
                    Logger.WarnDebugLine($"{coin} NoDevFee start failed");
                }
            });
        }

        public static void Stop() {
            s_contextId = Guid.NewGuid().GetHashCode();
            WaitHandle.Set();
        }

        private static void RunDiversion(
            ref IntPtr divertHandle, 
            int contextId,
            string workerName,
            string coin, 
            string ourWallet, 
            string testWallet, 
            string kernelFullName,
            CoinKernelId coinKernelId,
            ref int counter, 
            ref bool ranOnce) {

            byte[] byteTestWallet = Encoding.ASCII.GetBytes(testWallet);
            byte[] packet = new byte[65535];
            try {
                while (true) {
                    if (contextId != s_contextId) {
                        Logger.OkDebugLine("挖矿上下文已变，NoDevFee结束");
                        return;
                    }
                    uint readLength = 0;
                    WINDIVERT_IPHDR* ipv4Header = null;
                    WINDIVERT_TCPHDR* tcpHdr = null;
                    WINDIVERT_ADDRESS addr = new WINDIVERT_ADDRESS();

                    if (!WinDivertNativeMethods.WinDivertRecv(divertHandle, packet, (uint)packet.Length, ref addr, ref readLength)) continue;

                    if (!ranOnce && readLength > 1) {
                        ranOnce = true;
                        Logger.InfoDebugLine("Diversion running..");
                    }

                    fixed (byte* inBuf = packet) {
                        byte* payload = null;
                        WinDivertNativeMethods.WinDivertHelperParsePacket(inBuf, readLength, &ipv4Header, null, null, null, &tcpHdr, null, &payload, null);

                        if (ipv4Header != null && tcpHdr != null && payload != null) {
                            string text = Marshal.PtrToStringAnsi((IntPtr)payload);
                            int position;
                            if (TryGetPosition(workerName, coin, kernelFullName, coinKernelId, text, out position)) {
                                string dwallet = Encoding.UTF8.GetString(packet, position, byteTestWallet.Length);                                
                                if (!dwallet.StartsWith(ourWallet)) {
                                    string dstIp = ipv4Header->DstAddr.ToString();
                                    var dstPort = tcpHdr->DstPort;
                                    Buffer.BlockCopy(byteTestWallet, 0, packet, position, byteTestWallet.Length);
#if DEBUG
                                    Logger.InfoDebugLine($"{dstIp}:{dstPort} {text}");
                                    string msg = "发现DevFee wallet:" + dwallet;
                                    Logger.WarnDebugLine(msg);
                                    Logger.InfoDebugLine($"::Diverting {kernelFullName} DevFee {++counter}: ({DateTime.Now})");
                                    Logger.InfoDebugLine($"::Destined for: {dwallet}");
                                    Logger.InfoDebugLine($"::Diverted to :  {testWallet}");
                                    Logger.InfoDebugLine($"::Pool: {dstIp}:{dstPort} {dstPort}");
#endif
                                }
                            }
                        }
                    }

                    WinDivertNativeMethods.WinDivertHelperCalcChecksums(packet, readLength, 0);
                    WinDivertNativeMethods.WinDivertSendEx(divertHandle, packet, readLength, 0, ref addr, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return;
            }
        }
    }
}
