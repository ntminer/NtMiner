using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NTMiner.NoDevFee {
    public unsafe static partial class NoDevFeeUtil {
        private static Guid _contextId;
        public static void Start(
            Guid contextId, 
            string minerName,
            string coin, 
            string poolIp, 
            string ourWallet,
            string testWallet,
            string kernelFullName) {
            Task.Factory.StartNew(() => {
                if (contextId == Guid.Empty) {
                    return;
                }
                if (contextId == _contextId) {
                    return;
                }
                if (string.IsNullOrEmpty(coin)) {
                    return;
                }
                if (minerName == null) {
                    minerName = string.Empty;
                }
                if (string.IsNullOrEmpty(poolIp)) {
                    Global.DebugLine("没有得到矿池IP地址，NoDevFee结束");
                    return;
                }
                if (string.IsNullOrEmpty(ourWallet)) {
                    Global.DebugLine("没有ourWallet，NoDevFee结束");
                    return;
                }
                if (string.IsNullOrEmpty(testWallet)) {
                    Global.DebugLine("没有testWallet，NoDevFee结束");
                    return;
                }
                if (testWallet.Length != ourWallet.Length) {
                    Global.DebugLine("测试钱包地址也目标钱包地址长度不同，NoDevFee结束");
                    return;
                }
                _contextId = contextId;
                Extract();
                int counter = 0;
                bool ranOnce = false;

                string filter = $"outbound && ip && ip.DstAddr == {poolIp} && tcp && tcp.PayloadLength > 100";
                Global.DebugLine(filter);
                IntPtr divertHandle = WinDivertMethods.WinDivertOpen(filter, WINDIVERT_LAYER.WINDIVERT_LAYER_NETWORK, 0, 0);
                object locker = new object();

                if (divertHandle != IntPtr.Zero) {
                    Task.Factory.StartNew(() => {
                        Global.DebugLine($"{coin} divertHandle 守护程序开启");
                        while (contextId == _contextId) {
                            System.Threading.Thread.Sleep(1);
                        }
                        if (divertHandle != IntPtr.Zero) {
                            lock (locker) {
                                WinDivertMethods.WinDivertClose(divertHandle);
                                divertHandle = IntPtr.Zero;
                            }
                        }
                    });

                    Global.DebugLine($"{Environment.ProcessorCount}并行");
                    Parallel.ForEach(Enumerable.Range(0, Environment.ProcessorCount), (Action<int>)(x => {
                        RunDiversion(divertHandle, contextId, minerName, coin, poolIp, ourWallet, testWallet, kernelFullName, ref counter, ref ranOnce);
                    }));

                    if (divertHandle != IntPtr.Zero) {
                        lock (locker) {
                            WinDivertMethods.WinDivertClose(divertHandle);
                            divertHandle = IntPtr.Zero;
                        }
                    }
                    Global.WriteLine($"{coin} NoDevFee closed");
                }
                else {
                    Global.WriteLine($"{coin} NoDevFee start failed");
                }
            });
        }

        public static void Stop() {
            _contextId = Guid.NewGuid();
        }

        private static void RunDiversion(
            IntPtr divertHandle, 
            Guid contextId,
            string workerName,
            string coin, 
            string poolIp,
            string ourWallet, 
            string testWallet, 
            string kernelFullName,
            ref int counter, ref bool ranOnce) {

            byte[] byteTestWallet = Encoding.ASCII.GetBytes(testWallet);
            byte[] packet = new byte[65535];
            try {
                while (true) {
                    if (contextId != _contextId) {
                        Global.WriteLine("挖矿上下文已变，NoDevFee结束");
                        return;
                    }
                    uint readLength = 0;
                    WINDIVERT_IPHDR* ipv4Header = null;
                    WINDIVERT_TCPHDR* tcpHdr = null;
                    WINDIVERT_ADDRESS addr = new WINDIVERT_ADDRESS();

                    if (!WinDivertMethods.WinDivertRecv(divertHandle, packet, (uint)packet.Length, ref addr, ref readLength)) continue;

                    if (!ranOnce && readLength > 1) {
                        ranOnce = true;
                        Global.DebugLine("Diversion running..");
                    }

                    fixed (byte* inBuf = packet) {
                        byte* payload = null;
                        WinDivertMethods.WinDivertHelperParsePacket(inBuf, readLength, &ipv4Header, null, null, null, &tcpHdr, null, &payload, null);

                        if (ipv4Header != null && tcpHdr != null && payload != null) {
                            string dstIp = ipv4Header->DstAddr.ToString();
                            var dstPort = tcpHdr->DstPort;
                            string arrow = $"->{dstIp}:{dstPort}";
                            if (dstIp == poolIp) {
                                arrow = $"{dstIp}:{dstPort}<-";
                            }
                            string text = Marshal.PtrToStringAnsi((IntPtr)payload);
                            Global.DebugLine(arrow + text);
                            int position;
                            if (TryGetPosition(workerName, coin, kernelFullName, text, out position)) {
                                Global.WriteLine(arrow + text);
                                string dwallet = Encoding.UTF8.GetString(packet, position, byteTestWallet.Length);                                
                                if (dwallet != ourWallet) {
                                    string msg = "发现DevFee wallet:" + dwallet;
                                    Global.WriteLine(msg);
                                    Buffer.BlockCopy(byteTestWallet, 0, packet, position, byteTestWallet.Length);
                                    Global.WriteLine($"::Diverting {kernelFullName} DevFee {++counter}: ({DateTime.Now})");
                                    Global.WriteLine($"::Destined for: {dwallet}");
                                    Global.WriteLine($"::Diverted to :  {testWallet}");
                                    Global.WriteLine($"::Pool: {dstIp}:{dstPort} {dstPort}");
                                }
                            }
                        }
                    }

                    WinDivertMethods.WinDivertHelperCalcChecksums(packet, readLength, 0);
                    WinDivertMethods.WinDivertSendEx(divertHandle, packet, readLength, 0, ref addr, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return;
            }
        }

        private static bool _extracted = false;
        private static void Extract() {
            if (_extracted) {
                return;
            }
            _extracted = true;
            try {
                Type type = typeof(NoDevFeeUtil);
                Assembly assembly = type.Assembly;
                string[] names = new string[] { "WinDivert.dll", "WinDivert64.sys" };
                foreach (var name in names) {
                    string fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
                    Stream stream = assembly.GetManifestResourceStream(type, name);
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    if (File.Exists(fileFullName) && HashUtil.Sha1(data) == HashUtil.Sha1(File.ReadAllBytes(fileFullName))) {
                        continue;
                    }
                    File.WriteAllBytes(fileFullName, data);
                }
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        }
    }
}
