using NTMiner.Core;
using NTMiner.Core.Kernels;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class NTMinerRoot : INTMinerRoot {
        private static class MinerProcess {
            #region CreateProcess
            public static void CreateProcessAsync(IMineContext mineContext) {
                Task.Factory.StartNew(() => {
                    try {
                        Windows.TaskKill.Kill(mineContext.Kernel.GetProcessName());
                        Thread.Sleep(1000);
                        Logger.InfoDebugLine("解压内核包");
                        // 解压内核包
                        mineContext.Kernel.ExtractPackage();

                        string kernelExeFileFullName;
                        string arguments;
                        Logger.InfoDebugLine("组装命令");
                        // 组装命令
                        BuildCmdLine(mineContext, out kernelExeFileFullName, out arguments);
                        bool isLogFile = arguments.Contains("{logfile}");
                        // 这是不应该发生的，如果发生很可能是填写命令的时候拼写错误了
                        if (!File.Exists(kernelExeFileFullName)) {
                            Logger.ErrorDebugLine(kernelExeFileFullName + "文件不存在，请检查是否有拼写错误");
                        }
                        if (isLogFile) {
                            Logger.InfoDebugLine("创建日志文件型进程");
                            // 如果内核支持日志文件
                            // 推迟打印cmdLine，因为{logfile}变量尚未求值
                            CreateLogfileProcess(mineContext, kernelExeFileFullName, arguments);
                        }
                        else {
                            Logger.InfoDebugLine("创建管道型进程");
                            // 如果内核不支持日志文件
                            string cmdLine = $"\"{kernelExeFileFullName}\" {arguments}";
                            Logger.InfoDebugLine(cmdLine);
                            CreatePipProcess(mineContext, cmdLine);
                        }
                        VirtualRoot.Happened(new MineStartedEvent(mineContext));
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
            #endregion

            #region BuildCmdLine
            private static void BuildCmdLine(IMineContext mineContext, out string kernelExeFileFullName, out string arguments) {
                var kernel = mineContext.Kernel;
                string kernelDir = Path.Combine(SpecialPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(kernel.Package));
                string kernelCommandName = kernel.GetCommandName();
                kernelExeFileFullName = Path.Combine(kernelDir, kernelCommandName);
                if (!kernelExeFileFullName.EndsWith(".exe")) {
                    kernelExeFileFullName += ".exe";
                }
                var args = mineContext.CommandLine;
                arguments = args.Substring(kernelCommandName.Length).Trim();
            }
            #endregion

            #region Daemon
            private static Bus.DelegateHandler<Per1MinuteEvent> s_daemon = null;
            private static void Daemon(IMineContext mineContext, Action clear) {
                if (s_daemon != null) {
                    VirtualRoot.UnPath(s_daemon);
                    s_daemon = null;
                    clear?.Invoke();
                }
                string processName = mineContext.Kernel.GetProcessName();
                s_daemon = VirtualRoot.On<Per1MinuteEvent>("周期性检查挖矿内核是否消失，如果消失尝试重启", LogEnum.DevConsole,
                    action: message => {
                        if (mineContext == Current.CurrentMineContext) {
                            if (!string.IsNullOrEmpty(processName)) {
                                Process[] processes = Process.GetProcessesByName(processName);
                                if (processes.Length == 0) {
                                    mineContext.ProcessDisappearedCound = mineContext.ProcessDisappearedCound + 1;
                                    Logger.ErrorWriteLine(processName + $"挖矿内核进程消失");
                                    if (Current.MinerProfile.IsAutoRestartKernel && mineContext.ProcessDisappearedCound <= 3) {
                                        Logger.WarnWriteLine($"尝试第{mineContext.ProcessDisappearedCound}次重启，共3次");
                                        Current.RestartMine();
                                        Current.CurrentMineContext.ProcessDisappearedCound = mineContext.ProcessDisappearedCound;
                                    }
                                    else {
                                        Current.StopMineAsync();
                                    }
                                    if (s_daemon != null) {
                                        VirtualRoot.UnPath(s_daemon);
                                        clear?.Invoke();
                                    }
                                }
                            }
                        }
                        else {
                            if (s_daemon != null) {
                                VirtualRoot.UnPath(s_daemon);
                                clear?.Invoke();
                            }
                        }
                    });
            }
            #endregion

            #region CreateLogfileProcess
            private static void CreateLogfileProcess(IMineContext mineContext, string kernelExeFileFullName, string arguments) {
                string logFile = Path.Combine(SpecialPath.LogsDirFullName, "logfile_" + DateTime.Now.Ticks.ToString() + ".log");
                arguments = arguments.Replace("{logfile}", logFile);
                string cmdLine = $"\"{kernelExeFileFullName}\" {arguments}";
                Logger.InfoDebugLine(cmdLine);
                ProcessStartInfo startInfo = new ProcessStartInfo(kernelExeFileFullName, arguments) {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.Combine(SpecialPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(mineContext.Kernel.Package))
                };
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                ReadPrintLoopLogFileAsync(mineContext, logFile);
                Daemon(mineContext, null);
            }
            #endregion

            #region ReadPrintLoopLogFile
            private static void ReadPrintLoopLogFileAsync(IMineContext mineContext, string logFile) {
                Task.Factory.StartNew(() => {
                    bool isLogFileCreated = true;
                    int n = 0;
                    while (!File.Exists(logFile)) {
                        if (n >= 10) {
                            // 10秒钟都没有建立日志文件，不可能
                            isLogFileCreated = false;
                            break;
                        }
                        Thread.Sleep(1000);
                        if (mineContext != Current.CurrentMineContext) {
                            isLogFileCreated = false;
                            break;
                        }
                        n++;
                    }
                    if (isLogFileCreated) {
                        FileStream stream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (StreamReader sreader = new StreamReader(stream, Encoding.Default)) {
                            while (mineContext == Current.CurrentMineContext) {
                                string outline = sreader.ReadLine();
                                if (string.IsNullOrEmpty(outline)) {
                                    Thread.Sleep(1000);
                                }
                                else {
                                    string input = outline;
                                    Guid kernelOutputId = mineContext.Kernel.KernelOutputId;
                                    Current.KernelOutputFilterSet.Filter(kernelOutputId, ref input);
                                    ConsoleColor color = ConsoleColor.White;
                                    Current.KernelOutputTranslaterSet.Translate(kernelOutputId, ref input, ref color, isPre: true);
                                    // 使用Claymore挖其非ETH币种时它也打印ETH，所以这里需要纠正它
                                    if ("Claymore".Equals(mineContext.Kernel.Code, StringComparison.OrdinalIgnoreCase)) {
                                        if (mineContext.MainCoin.Code != "ETH" && input.Contains("ETH")) {
                                            input = input.Replace("ETH", mineContext.MainCoin.Code);
                                        }
                                    }
                                    Current.KernelOutputSet.Pick(kernelOutputId, ref input, mineContext);
                                    Current.KernelOutputTranslaterSet.Translate(kernelOutputId, ref input, ref color);
                                    if (!string.IsNullOrEmpty(input)) {
                                        IKernelOutput kernelOutput;
                                        if (Current.KernelOutputSet.TryGetKernelOutput(kernelOutputId, out kernelOutput)) {
                                            if (kernelOutput.PrependDateTime) {
                                                Write.UserLine($"{DateTime.Now}    {input}", color);
                                            }
                                            else {
                                                Write.UserLine(input, color);
                                            }
                                        }
                                        else {
                                            Write.UserLine(input, color);
                                        }
                                    }
                                }
                            }
                        }
                        Logger.WarnWriteLine("日志输出结束");
                    }
                });
            }
            #endregion

            #region CreatePipProcess
            // 创建管道，将输出通过管道转送到日志文件，然后读取日志文件内容打印到控制台
            private static void CreatePipProcess(IMineContext mineContext, string cmdLine) {
                SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES();
                IntPtr hReadOut, hWriteOut;

                //set the bInheritHandle flag so pipe handles are inherited

                saAttr.bInheritHandle = true;
                saAttr.lpSecurityDescriptor = IntPtr.Zero;
                saAttr.length = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                saAttr.lpSecurityDescriptor = IntPtr.Zero;
                //get handle to current stdOut

                bool bret;

                IntPtr mypointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(STARTUPINFO)));
                Marshal.StructureToPtr(saAttr, mypointer, true);
                bret = CreatePipe(out hReadOut, out hWriteOut, mypointer, 0);
                //ensure the read handle to pipe for stdout is not inherited
                SetHandleInformation(hReadOut, HANDLE_FLAG_INHERIT, 0);
                ////Create pipe for the child process's STDIN
                STARTUPINFO lpStartupInfo = new STARTUPINFO {
                    cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
                    dwFlags = STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW,
                    wShowWindow = SW_HIDE, // SW_HIDE; //SW_SHOW
                    hStdOutput = hWriteOut,
                    hStdError = hWriteOut,
                    hStdInput = IntPtr.Zero
                };
                PROCESS_INFORMATION lpProcessInformation;
                if (CreateProcess(
                    lpApplicationName: null,
                    lpCommandLine: new StringBuilder(cmdLine),
                    lpProcessAttributes: IntPtr.Zero,
                    lpThreadAttributes: IntPtr.Zero,
                    bInheritHandles: true,
                    dwCreationFlags: NORMAL_PRIORITY_CLASS,
                    lpEnvironment: IntPtr.Zero,
                    lpCurrentDirectory: null,
                    lpStartupInfo: ref lpStartupInfo,
                    lpProcessInformation: out lpProcessInformation)) {
                    if (bret == false) {
                        int lasterr = Marshal.GetLastWin32Error();
                    }
                    else {
                        Bus.DelegateHandler<MineStopedEvent> closeHandle = null;
                        bool isHWriteOutHasClosed = false;
                        Daemon(mineContext, () => {
                            if (!isHWriteOutHasClosed) {
                                CloseHandle(hWriteOut);
                                isHWriteOutHasClosed = true;
                            }
                            VirtualRoot.UnPath(closeHandle);
                        });
                        closeHandle = VirtualRoot.On<MineStopedEvent>("挖矿停止后关闭非托管的日志句柄", LogEnum.DevConsole,
                            action: message => {
                                // 挖矿停止后摘除挖矿内核进程守护器
                                if (s_daemon != null) {
                                    VirtualRoot.UnPath(s_daemon);
                                    s_daemon = null;
                                }
                                if (!isHWriteOutHasClosed) {
                                    CloseHandle(hWriteOut);
                                    isHWriteOutHasClosed = true;
                                }
                                VirtualRoot.UnPath(closeHandle);
                            });
                        string pipLogFileFullName = Path.Combine(SpecialPath.LogsDirFullName, mineContext.PipeFileName);
                        Task.Factory.StartNew(() => {
                            FileStream fs = new FileStream(pipLogFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            using (StreamReader sr = new StreamReader(fs)) {
                                byte[] buffer = new byte[1024];
                                int ret;
                                // Read会阻塞，直到读取到字符或者hWriteOut被关闭
                                while ((ret = Read(buffer, 0, buffer.Length, hReadOut)) > 0) {
                                    fs.Write(buffer, 0, ret);
                                    if (buffer[ret - 1] == '\r' || buffer[ret - 1] == '\n') {
                                        fs.Flush();
                                    }
                                }
                            }
                            CloseHandle(hReadOut);
                        });
                        ReadPrintLoopLogFileAsync(mineContext, pipLogFileFullName);
                    }
                }
                else {
                    Logger.ErrorWriteLine("内核启动失败，请重试");
                }
            }

            private struct STARTUPINFO {
                public uint cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public uint dwX;
                public uint dwY;
                public uint dwXSize;
                public uint dwYSize;
                public uint dwXCountChars;
                public uint dwYCountChars;
                public uint dwFillAttribute;
                public uint dwFlags;
                public short wShowWindow;
                public short cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            private struct PROCESS_INFORMATION {
                public IntPtr hProcess;
                public IntPtr hThread;
                public uint dwProcessId;
                public uint dwThreadId;
            }

            private struct SECURITY_ATTRIBUTES {
                public int length;
                public IntPtr lpSecurityDescriptor;
                public bool bInheritHandle;
            }

            [DllImport("kernel32.dll")]
            private static extern int CloseHandle(IntPtr hObject);

            [DllImport("kernel32.dll")]
            private static extern bool CreatePipe(out IntPtr phReadPipe, out IntPtr phWritePipe, IntPtr lpPipeAttributes, uint nSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern unsafe bool ReadFile(
                IntPtr hfile,
                void* pBuffer,
                int NumberOfBytesToRead,
                int* pNumberOfBytesRead,
                NativeOverlapped* lpOverlapped
            );
            [DllImport("kernel32.dll")]
            private static extern bool CreateProcess(
                string lpApplicationName,
                StringBuilder lpCommandLine,
                IntPtr lpProcessAttributes,
                IntPtr lpThreadAttributes,
                bool bInheritHandles,
                uint dwCreationFlags,
                IntPtr lpEnvironment,
                string lpCurrentDirectory,
                ref STARTUPINFO lpStartupInfo,
                out PROCESS_INFORMATION lpProcessInformation);

            [DllImport("kernel32.dll")]
            private static extern bool SetHandleInformation(IntPtr hObject, int dwMask, uint dwFlags);

            private static unsafe int Read(byte[] buffer, int index, int count, IntPtr hStdOut) {
                int n = 0;
                fixed (byte* p = buffer) {
                    if (!ReadFile(hStdOut, p + index, count, &n, (NativeOverlapped*)0))
                        return 0;
                }
                return n;
            }

            private const uint STARTF_USESHOWWINDOW = 0x00000001;
            private const uint STARTF_USESTDHANDLES = 0x00000100;
            private const uint STARTF_FORCEONFEEDBACK = 0x00000040;
            private const uint SF_USEPOSITION = 0x00000004;
            private const uint STARTF_USESIZE = 0x00000002;
            private const uint STARTF_USECOUNTCHARS = 0x00000008;
            private const uint NORMAL_PRIORITY_CLASS = 0x00000020;
            private const uint CREATE_BREAKAWAY_FROM_JOB = 0x01000000;
            private const uint CREATE_NO_WINDOW = 0x08000000;
            private const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
            private const short SW_SHOW = 5;
            private const short SW_HIDE = 0;
            private const int STD_OUTPUT_HANDLE = -11;
            private const int HANDLE_FLAG_INHERIT = 1;
            private const uint GENERIC_READ = 0x80000000;
            private const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
            private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
            private const int OPEN_EXISTING = 3;
            private const uint CREATE_NEW_CONSOLE = 0x00000010;
            private const uint STILL_ACTIVE = 0x00000103;
            #endregion
        }
    }
}
