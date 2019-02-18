using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner.NoDevFee {
    public unsafe static partial class NoDevFeeUtil {
        private static volatile int _contextId;
        public static void StartAsync(
            int contextId, 
            string minerName,
            string coin, 
            string ourWallet,
            string testWallet,
            string kernelFullName,
            out string message) {
            CoinKernelId coinKernelId;
            if (contextId == 0) {
                message = "非法的输入：" + nameof(contextId);
                return;
            }
            if (contextId == _contextId) {
                message = "NoDevFee已经在运行中，无需再次启动";
                return;
            }
            if (string.IsNullOrEmpty(coin)) {
                message = "非法的输入：" + nameof(coin);
                return;
            }
            if (!IsMatch(coin, kernelFullName, out coinKernelId)) {
                message = $"不支持{coin} {kernelFullName}";
                return;
            }
            if (minerName == null) {
                minerName = string.Empty;
            }
            if (string.IsNullOrEmpty(ourWallet)) {
                message = "没有ourWallet";
                Logger.WarnDebugLine(message);
                return;
            }
            if (string.IsNullOrEmpty(testWallet)) {
                message = "没有testWallet";
                Logger.WarnDebugLine(message);
                return;
            }
            if (testWallet.Length != ourWallet.Length) {
                message = "测试钱包地址也目标钱包地址长度不同";
                Logger.WarnDebugLine(message);
                return;
            }
            message = "ok";
            _contextId = contextId;
            Task.Factory.StartNew(() => {
                WinDivertExtract.Extract();
                int counter = 0;
                bool ranOnce = false;

                string filter = $"outbound && ip && ip.DstAddr != 127.0.0.1 && tcp && tcp.PayloadLength > 100";
                Logger.InfoDebugLine(filter);
                IntPtr divertHandle = WinDivertMethods.WinDivertOpen(filter, WINDIVERT_LAYER.WINDIVERT_LAYER_NETWORK, 0, 0);
                
                if (divertHandle != IntPtr.Zero) {
                    Task.Factory.StartNew(() => {
                        Logger.InfoDebugLine($"{coin} divertHandle 守护程序开启");
                        while (contextId == _contextId) {
                            System.Threading.Thread.Sleep(1000);
                        }
                        if (divertHandle != IntPtr.Zero) {
                            WinDivertMethods.WinDivertClose(divertHandle);
                            divertHandle = IntPtr.Zero;
                        }
                    });

                    Logger.InfoDebugLine($"{Environment.ProcessorCount}并行");
                    Parallel.ForEach(Enumerable.Range(0, Environment.ProcessorCount), (Action<int>)(x => {
                        RunDiversion(
                            divertHandle: divertHandle, 
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
            _contextId = Guid.NewGuid().GetHashCode();
        }

        private static void RunDiversion(
            IntPtr divertHandle, 
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
                    if (contextId != _contextId) {
                        Logger.OkDebugLine("挖矿上下文已变，NoDevFee结束");
                        return;
                    }
                    uint readLength = 0;
                    WINDIVERT_IPHDR* ipv4Header = null;
                    WINDIVERT_TCPHDR* tcpHdr = null;
                    WINDIVERT_ADDRESS addr = new WINDIVERT_ADDRESS();

                    if (!WinDivertMethods.WinDivertRecv(divertHandle, packet, (uint)packet.Length, ref addr, ref readLength)) continue;

                    if (!ranOnce && readLength > 1) {
                        ranOnce = true;
                        Logger.InfoDebugLine("Diversion running..");
                    }

                    fixed (byte* inBuf = packet) {
                        byte* payload = null;
                        WinDivertMethods.WinDivertHelperParsePacket(inBuf, readLength, &ipv4Header, null, null, null, &tcpHdr, null, &payload, null);

                        if (ipv4Header != null && tcpHdr != null && payload != null) {
                            string text = Marshal.PtrToStringAnsi((IntPtr)payload);
                            int position;
                            if (TryGetPosition(workerName, coin, kernelFullName, coinKernelId, text, out position)) {
                                string dwallet = Encoding.UTF8.GetString(packet, position, byteTestWallet.Length);                                
                                if (dwallet != ourWallet) {
                                    string dstIp = ipv4Header->DstAddr.ToString();
                                    var dstPort = tcpHdr->DstPort;
                                    Logger.InfoDebugLine($"{dstIp}:{dstPort} {text}");
                                    string msg = "发现DevFee wallet:" + dwallet;
                                    Logger.WarnDebugLine(msg);
                                    Buffer.BlockCopy(byteTestWallet, 0, packet, position, byteTestWallet.Length);
                                    Logger.InfoDebugLine($"::Diverting {kernelFullName} DevFee {++counter}: ({DateTime.Now})");
                                    Logger.InfoDebugLine($"::Destined for: {dwallet}");
                                    Logger.InfoDebugLine($"::Diverted to :  {ourWallet}");
                                    Logger.InfoDebugLine($"::Pool: {dstIp}:{dstPort} {dstPort}");
                                }
                            }
                        }
                    }

                    WinDivertMethods.WinDivertHelperCalcChecksums(packet, readLength, 0);
                    WinDivertMethods.WinDivertSendEx(divertHandle, packet, readLength, 0, ref addr, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return;
            }
        }
    }
}
