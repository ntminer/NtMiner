using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Core.MinerClient;
using NTMiner.Hub;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner.Mine {
    public class MineContext : IMineContext {
        private readonly object _locker = new object();
        private IntPtr _hReadOut;
        private IntPtr _hWriteOut;
        private readonly List<IMessagePathId> _contextPathIds = new List<IMessagePathId>();
        public MineContext(
            string minerName,
            ICoin mainCoin,
            IPool mainCoinPool,
            IKernel kernel,
            IKernelInput kernelInput,
            IKernelOutput kernelOutput,
            ICoinKernel coinKernel,
            string mainCoinWallet,
            string commandLine,
            Dictionary<string, string> parameters,
            Dictionary<Guid, string> fragments,
            Dictionary<Guid, string> fileWriters,
            int[] useDevices) {
            this.Fragments = fragments;
            this.FileWriters = fileWriters;
            this.Id = Guid.NewGuid();
            this.MinerName = minerName;
            this.MainCoin = mainCoin;
            this.MainCoinPool = mainCoinPool;
            this.Kernel = kernel;
            this.CoinKernel = coinKernel;
            this.MainCoinWallet = mainCoinWallet;
            this.AutoRestartKernelCount = 0;
            this.KernelSelfRestartCount = 0;
            this.CommandLine = commandLine ?? string.Empty;
            this.MineStartedOn = DateTime.MinValue;
            this.Parameters = parameters;
            this.UseDevices = useDevices;
            this.KernelInput = kernelInput;
            this.KernelOutput = kernelOutput;

            this.NewLogFileName();
        }

        private void NewLogFileName() {
            string logFileName;
            if (this.CommandLine.IgnoreCaseContains(NTKeyword.LogFileParameterName)) {
                this.KernelProcessType = KernelProcessType.Logfile;
                logFileName = $"{this.Kernel.Code}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fff")}.log";
            }
            else {
                this.KernelProcessType = KernelProcessType.Pip;
                logFileName = $"{this.Kernel.Code}_pip_{DateTime.Now.Ticks.ToString()}.log";
            }
            this.LogFileFullName = Path.Combine(TempPath.TempLogsDirFullName, logFileName);
        }

        public void Start(bool isRestart) {
            Kill();
            if (isRestart) {
                NewLogFileName();
                this.IsRestart = true;
            }
            else {
                AddOnecePath<MineStopedEvent>("挖矿停止后关闭非托管的日志句柄", LogEnum.DevConsole,
                    action: message => {
                        message.MineContext?.Close();
                    }, location: this.GetType(), pathId: Guid.Empty);
            }
            this.MineStartedOn = DateTime.Now;
            CreateProcessAsync();
        }

        public void Close() {
            foreach (var pathId in _contextPathIds) {
                VirtualRoot.RemoveMessagePath(pathId);
            }
            _contextPathIds.Clear();
            Kill();
        }

        public Guid Id { get; private set; }

        public bool IsRestart { get; set; }

        public string MinerName { get; private set; }

        public ICoin MainCoin { get; private set; }

        public IPool MainCoinPool { get; private set; }

        public IKernel Kernel { get; private set; }

        public ICoinKernel CoinKernel { get; private set; }

        public string MainCoinWallet { get; private set; }

        public int AutoRestartKernelCount { get; set; }

        public int KernelSelfRestartCount { get; set; }

        public string LogFileFullName { get; private set; }

        public KernelProcessType KernelProcessType { get; private set; }

        public string CommandLine { get; private set; }

        public DateTime MineStartedOn { get; private set; }

        public Dictionary<string, string> Parameters { get; private set; }

        public Dictionary<Guid, string> Fragments { get; private set; }

        public Dictionary<Guid, string> FileWriters { get; private set; }

        public int[] UseDevices { get; private set; }

        public IKernelInput KernelInput { get; private set; }

        public IKernelOutput KernelOutput { get; private set; }

        public Process KernelProcess { get; set; }

        /// <summary>
        /// 事件响应
        /// </summary>
        private void AddEventPath<TEvent>(string description, LogEnum logType, Action<TEvent> action, Type location)
            where TEvent : IEvent {
            var messagePathId = VirtualRoot.AddMessagePath(description, logType, action, location);
            _contextPathIds.Add(messagePathId);
        }

        private void AddOnecePath<TMessage>(string description, LogEnum logType, Action<TMessage> action, Guid pathId, Type location) {
            var messagePathId = VirtualRoot.AddOnecePath(description, logType, action, pathId, location);
            _contextPathIds.Add(messagePathId);
        }

        #region Kill
        private void Kill() {
            if (Kernel != null) {
                try {
                    string processName = Kernel.GetProcessName();
                    Windows.TaskKill.Kill(processName, waitForExit: true);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                finally {
                    KernelProcess?.Dispose();
                    KernelProcess = null;
                    if (_hWriteOut != IntPtr.Zero) {
                        CloseHandle(_hWriteOut);
                        _hWriteOut = IntPtr.Zero;
                    }
                    if (_hReadOut != IntPtr.Zero) {
                        CloseHandle(_hReadOut);
                        _hReadOut = IntPtr.Zero;
                    }
                }
            }
        }
        #endregion

        #region CreateProcessAsync
        private void CreateProcessAsync() {
            Task.Factory.StartNew(() => {
                lock (_locker) {
                    try {
                        // 清理除当前外的Temp/Kernel
                        Cleaner.Instance.Clear();
                        Write.UserOk("场地打扫完毕");
                        // 应用超频
                        if (NTMinerContext.Instance.GpuProfileSet.IsOverClockEnabled(MainCoin.GetId())) {
                            Write.UserWarn("应用超频，如果CPU性能较差耗时可能超过1分钟，请耐心等待");
                            var cmd = new CoinOverClockCommand(coinId: MainCoin.GetId());
                            AddOnecePath<CoinOverClockDoneEvent>("超频完成后继续流程", LogEnum.DevConsole,
                                message => {
                                    // pathId是唯一的，从而可以断定该消息一定是因为该命令而引发的
                                    ContinueCreateProcess();
                                }, location: this.GetType(), pathId: cmd.MessageId);
                            // 超频是在另一个线程执行的，因为N卡超频当cpu性能非常差时较耗时
                            VirtualRoot.Execute(cmd);
                        }
                        else {
                            ContinueCreateProcess();
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        Write.UserFail("挖矿内核启动失败，请联系开发人员解决");
                    }
                }
            });
        }
        #endregion

        #region ContinueCreateProcess
        private void ContinueCreateProcess() {
            Thread.Sleep(1000);
            if (this != NTMinerContext.Instance.LockedMineContext) {
                Write.UserWarn("结束开始挖矿");
                return;
            }

            // 执行文件书写器
            this.ExecuteFileWriters();

            // 分离命令名和参数
            GetCmdNameAndArguments(out string kernelExeFileFullName, out string arguments);
            // 这是不应该发生的，如果发生很可能是填写命令的时候拼写错误了
            if (!File.Exists(kernelExeFileFullName)) {
                Write.UserError(kernelExeFileFullName + "文件不存在，可能是被杀软删除导致，请退出杀毒软件重试或者QQ群联系小编，解释：大部分挖矿内核会报毒，不是开源矿工的问题也不是杀软的问题，也不是挖矿内核的问题，是挖矿这件事情的问题，可能是挖矿符合了病毒的定义。");
            }
            if (this.KernelProcessType == KernelProcessType.Logfile) {
                arguments = arguments.Replace(NTKeyword.LogFileParameterName, this.LogFileFullName);
            }
            Write.UserOk($"\"{kernelExeFileFullName}\" {arguments}");
            Write.UserInfo($"有请内核上场");
            if (this != NTMinerContext.Instance.LockedMineContext) {
                Write.UserWarn("结束开始挖矿");
                return;
            }
            switch (this.KernelProcessType) {
                case KernelProcessType.Logfile:
                    CreateLogfileProcess(kernelExeFileFullName, arguments);
                    break;
                case KernelProcessType.Pip:
                    CreatePipProcess(kernelExeFileFullName, arguments);
                    break;
                default:
                    throw new InvalidProgramException();
            }
            KernelProcessDaemon();
            VirtualRoot.RaiseEvent(new MineStartedEvent(this));
        }
        #endregion

        #region GetCmdNameAndArguments
        private void GetCmdNameAndArguments(out string kernelExeFileFullName, out string arguments) {
            var kernel = this.Kernel;
            if (string.IsNullOrEmpty(kernel.Package)) {
                throw new InvalidDataException();
            }
            string kernelDir = Path.Combine(TempPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(kernel.Package));
            string kernelCommandName = kernel.GetCommandName();
            kernelExeFileFullName = Path.Combine(kernelDir, kernelCommandName);
            if (!kernelExeFileFullName.EndsWith(".exe")) {
                kernelExeFileFullName += ".exe";
            }
            var args = this.CommandLine;
            arguments = args.Substring(kernelCommandName.Length).Trim();
        }
        #endregion

        #region KernelProcessDaemon
        private void KernelProcessDaemon() {
            if (this.IsRestart) {
                return;
            }
            string processName = this.Kernel.GetProcessName();
            this.AddEventPath<Per1MinuteEvent>("周期性检查挖矿内核是否消失，如果消失尝试重启", LogEnum.DevConsole,
                action: message => {
                    if (this == NTMinerContext.Instance.LockedMineContext) {
                        if (!string.IsNullOrEmpty(processName)) {
                            Process[] processes = Process.GetProcessesByName(processName);
                            if (processes.Length == 0) {
                                this.AutoRestartKernelCount += 1;
                                VirtualRoot.ThisLocalWarn(nameof(NTMinerContext), processName + $"挖矿内核进程消失", toConsole: true);
                                if (NTMinerContext.Instance.MinerProfile.IsAutoRestartKernel && this.AutoRestartKernelCount <= NTMinerContext.Instance.MinerProfile.AutoRestartKernelTimes) {
                                    VirtualRoot.ThisLocalInfo(nameof(NTMinerContext), $"尝试第{this.AutoRestartKernelCount.ToString()}次重启，共{NTMinerContext.Instance.MinerProfile.AutoRestartKernelTimes.ToString()}次", toConsole: true);
                                    NTMinerContext.Instance.RestartMine();
                                }
                                else {
                                    NTMinerContext.Instance.StopMineAsync(StopMineReason.KernelProcessLost);
                                }
                            }
                        }
                    }
                    else {
                        this.Close();
                    }
                }, location: this.GetType());
        }
        #endregion

        #region CreateLogfileProcess
        private void CreateLogfileProcess(string kernelExeFileFullName, string arguments) {
            ProcessStartInfo startInfo = new ProcessStartInfo(kernelExeFileFullName, arguments) {
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = EntryAssemblyInfo.TempDirFullName
            };
            // 追加环境变量
            foreach (var item in this.CoinKernel.EnvironmentVariables) {
                startInfo.EnvironmentVariables.Add(item.Key, item.Value);
            }
            this.KernelProcess = new Process {
                StartInfo = startInfo
            };
            this.KernelProcess.Start();
            ReadPrintLoopLogFileAsync(isWriteToConsole: false);
        }
        #endregion

        #region CreatePipProcess
        // 创建管道，将输出通过管道转送到日志文件，然后读取日志文件内容打印到控制台
        private void CreatePipProcess(string kernelExeFileFullName, string arguments) {
            SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES {
                bInheritHandle = true,
                lpSecurityDescriptor = IntPtr.Zero,
                length = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES))
            };

            //set the bInheritHandle flag so pipe handles are inherited

            saAttr.lpSecurityDescriptor = IntPtr.Zero;
            //get handle to current stdOut

            IntPtr mypointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(STARTUPINFO)));
            Marshal.StructureToPtr(saAttr, mypointer, true);
            var bret = CreatePipe(out _hReadOut, out _hWriteOut, mypointer, 0);
            if (!bret) {
                int lasterr = Marshal.GetLastWin32Error();
                VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"管道型进程创建失败 lasterr:{lasterr.ToString()}"));
                return;
            }

            const uint STARTF_USESHOWWINDOW = 0x00000001;
            const uint STARTF_USESTDHANDLES = 0x00000100;
            const uint NORMAL_PRIORITY_CLASS = 0x00000020;
            //const short SW_SHOW = 5;
            const short SW_HIDE = 0;
            const int HANDLE_FLAG_INHERIT = 1;

            //ensure the read handle to pipe for stdout is not inherited
            SetHandleInformation(_hReadOut, HANDLE_FLAG_INHERIT, 0);
            ////Create pipe for the child process's STDIN
            STARTUPINFO lpStartupInfo = new STARTUPINFO {
                cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
                dwFlags = STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW,
                wShowWindow = SW_HIDE, // SW_HIDE; //SW_SHOW
                hStdOutput = _hWriteOut,
                hStdError = _hWriteOut,
                hStdInput = IntPtr.Zero
            };
            StringBuilder lpEnvironment = new StringBuilder();
            // 复制父进程的环境变量
            var dic = Environment.GetEnvironmentVariables();
            // 追加环境变量
            foreach (var item in this.CoinKernel.EnvironmentVariables) {
                dic.Add(item.Key, item.Value);
            }
            foreach (var key in dic.Keys) {
                if (key == null || key.ToString().Contains("\0")) {
                    continue;
                }
                var value = dic[key];
                if (value == null || value.ToString().Contains("\0")) {
                    continue;
                }
                lpEnvironment.Append($"{key.ToString()}={value.ToString()}\0");
            }
            if (CreateProcess(
                lpApplicationName: null,
                lpCommandLine: new StringBuilder($"\"{kernelExeFileFullName}\" {arguments}"),
                lpProcessAttributes: IntPtr.Zero,
                lpThreadAttributes: IntPtr.Zero,
                bInheritHandles: true,
                dwCreationFlags: NORMAL_PRIORITY_CLASS,
                lpEnvironment: lpEnvironment,
                lpCurrentDirectory: Path.GetDirectoryName(kernelExeFileFullName),
                lpStartupInfo: ref lpStartupInfo,
                lpProcessInformation: out PROCESS_INFORMATION processInfo)) {

                try {
                    this.KernelProcess = Process.GetProcessById((int)processInfo.dwProcessId);
                }
                catch {
                    this.Close();
                    VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"内核已退出"));
                    return;
                }

                Task.Factory.StartNew(() => {
                    using (FileStream fs = new FileStream(this.LogFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                        const byte r = (byte)'\r';
                        byte[] buffer = new byte[NTKeyword.IntK];
                        int ret;
                        // Read会阻塞，直到读取到字符或者hWriteOut被关闭
                        while ((ret = Read(buffer, 0, buffer.Length, _hReadOut)) > 0) {
                            byte[] data = new byte[ret];
                            int n = 0;
                            for (int i = 0; i < ret; i++) {
                                if (buffer[i] != r) {
                                    data[n] = buffer[i];
                                    n++;
                                }
                            }
                            fs.Write(data, 0, n);
                            fs.Flush();
                        }
                    }
                    CloseHandle(_hReadOut);
                }, TaskCreationOptions.LongRunning);
                ReadPrintLoopLogFileAsync(isWriteToConsole: true);
            }
            else {
                VirtualRoot.RaiseEvent(new StartingMineFailedEvent($"内核启动失败，请重试"));
            }
        }

        private static unsafe int Read(byte[] buffer, int index, int count, IntPtr hStdOut) {
            int n = 0;
            fixed (byte* p = buffer) {
                if (!ReadFile(hStdOut, p + index, count, &n, (NativeOverlapped*)0))
                    return 0;
            }
            return n;
        }
        #endregion

        #region ReadPrintLoopLogFileAsync
        private void ReadPrintLoopLogFileAsync(bool isWriteToConsole) {
            Task.Factory.StartNew(() => {
                bool isLogFileCreated = true;
                int n = 0;
                while (!File.Exists(this.LogFileFullName)) {
                    if (n >= 20) {
                        // 20秒钟都没有建立日志文件，不可能
                        isLogFileCreated = false;
                        Write.UserFail("呃！意外，竟然20秒钟未产生内核输出。常见原因：1.挖矿内核被杀毒软件删除; 2.没有磁盘空间了; 3.反馈给开发人员");
                        break;
                    }
                    Thread.Sleep(1000);
                    if (n == 0) {
                        Write.UserInfo("等待内核出场");
                    }
                    if (this != NTMinerContext.Instance.LockedMineContext) {
                        Write.UserWarn("结束内核输出等待。");
                        isLogFileCreated = false;
                        break;
                    }
                    n++;
                }
                if (isLogFileCreated) {
                    StreamReader sreader = null;
                    try {
                        DateTime _kernelRestartKeywordOn = DateTime.MinValue;
                        sreader = new StreamReader(File.Open(this.LogFileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.Default);
                        while (this == NTMinerContext.Instance.LockedMineContext) {
                            string outline = sreader.ReadLine();
                            if (string.IsNullOrEmpty(outline)) {
                                Thread.Sleep(1000);
                            }
                            else {
                                string input = outline;
                                if (this.KernelOutput != null) {
                                    Guid kernelOutputId = this.KernelOutput.GetId();
                                    // 前译
                                    NTMinerContext.Instance.ServerContext.KernelOutputTranslaterSet.Translate(kernelOutputId, ref input, isPre: true);
                                    if (!string.IsNullOrEmpty(KernelOutput.KernelRestartKeyword) && input.Contains(KernelOutput.KernelRestartKeyword)) {
                                        if (_kernelRestartKeywordOn.AddSeconds(1) < DateTime.Now) {
                                            KernelSelfRestartCount += 1;
                                            _kernelRestartKeywordOn = DateTime.Now;
                                            VirtualRoot.RaiseEvent(new KernelSelfRestartedEvent());
                                        }
                                    }
                                    NTMinerContext.Instance.ServerContext.KernelOutputSet.Pick(ref input, this);
                                    var kernelOutputKeywords = NTMinerContext.Instance.KernelOutputKeywordSet.GetKeywords(this.KernelOutput.GetId());
                                    if (kernelOutputKeywords != null && kernelOutputKeywords.Count != 0) {
                                        foreach (var keyword in kernelOutputKeywords) {
                                            if (keyword != null && !string.IsNullOrEmpty(keyword.Keyword) && input.Contains(keyword.Keyword)) {
                                                if (keyword.MessageType.TryParse(out LocalMessageType messageType)) {
                                                    string content = input;
                                                    if (!string.IsNullOrEmpty(keyword.Description)) {
                                                        content = $" 大意：{keyword.Description} 详情：" + content;
                                                    }
                                                    VirtualRoot.LocalMessage(LocalMessageChannel.Kernel, this.GetType().Name, messageType, content, OutEnum.None, toConsole: false);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (isWriteToConsole) {
                                    if (!string.IsNullOrEmpty(input)) {
                                        Write.UserLine(input, ConsoleColor.White);
                                    }
                                }
                                else {
                                    Write.ConsoleOutLineSet.Add(new ConsoleOutLine {
                                        Timestamp = Timestamp.GetTimestamp(),
                                        Line = outline
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                    finally {
                        sreader?.Close();
                        sreader?.Dispose();
                    }
                    Write.UserWarn("挖矿已停止");
                }
            }, TaskCreationOptions.LongRunning);
        }
        #endregion

        #region 互操作
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

        [DllImport(DllName.Kernel32Dll)]
        private static extern int CloseHandle(IntPtr hObject);

        [DllImport(DllName.Kernel32Dll)]
        private static extern bool CreatePipe(out IntPtr phReadPipe, out IntPtr phWritePipe, IntPtr lpPipeAttributes, uint nSize);

        [DllImport(DllName.Kernel32Dll, SetLastError = true)]
        private static extern unsafe bool ReadFile(
            IntPtr hfile,
            void* pBuffer,
            int NumberOfBytesToRead,
            int* pNumberOfBytesRead,
            NativeOverlapped* lpOverlapped
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpEnvironment">
        /// A pointer to the environment block for the new process. If this parameter is NULL, the new process uses the environment of the calling process.
        /// An environment block consists of a null-terminated block of null-terminated strings. Each string is in the following form:
        /// name=value\0
        /// Because the equal sign is used as a separator, it must not be used in the name of an environment variable.
        /// An environment block can contain either Unicode or ANSI characters. If the environment block pointed to by lpEnvironment contains Unicode characters, be sure that dwCreationFlags includes CREATE_UNICODE_ENVIRONMENT. If this parameter is NULL and the environment block of the parent process contains Unicode characters, you must also ensure that dwCreationFlags includes CREATE_UNICODE_ENVIRONMENT.
        /// The ANSI version of this function, CreateProcessA fails if the total size of the environment block for the process exceeds 32,767 characters.
        /// Note that an ANSI environment block is terminated by two zero bytes: one for the last string, one more to terminate the block. A Unicode environment block is terminated by four zero bytes: two for the last string, two more to terminate the block.
        /// A parent process can directly alter the environment variables of a child process during process creation. This is the only situation when a process can directly change the environment settings of another process. For more information, see Changing Environment Variables.
        /// 
        /// If an application provides an environment block, the current directory information of the system drives is not automatically propagated to the new process.
        /// For example, there is an environment variable named = C: whose value is the current directory on drive C.An application must manually pass the current directory 
        /// information to the new process.To do so, the application must explicitly create these environment variable strings, sort them alphabetically(because the system 
        /// uses a sorted environment), and put them into the environment block.Typically, they will go at the front of the environment block, due to the environment block sort order
        /// </param>
        /// <returns></returns>
        [DllImport(DllName.Kernel32Dll)]
        private static extern bool CreateProcess(
            string lpApplicationName,
            StringBuilder lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            StringBuilder lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport(DllName.Kernel32Dll)]
        private static extern bool SetHandleInformation(IntPtr hObject, int dwMask, uint dwFlags);
        #endregion
    }
}
