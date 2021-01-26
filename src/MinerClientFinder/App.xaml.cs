using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Windows;

namespace NTMiner {
    public class Program {
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main() {
            if (AppUtil.IsDotNetVersionEG45) {
                NTMiner.App app = new NTMiner.App();
                app.InitializeComponent();
                app.Run();
            }
            else {
                Process.Start("https://ntminer.com/getDotNet.html");
            }
            // 这个机制在MinerClient程序起作用但在MinerStudio程序中会发生类型初始化错误不起作用，具体原因未知
        }
    }

    public partial class App : Application {
        public App() {
            Logger.Disable();
            NTMinerConsole.Disable();
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            WpfUtil.Init();
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnExit(ExitEventArgs e) {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            if (AppUtil.GetMutex(NTKeyword.MinerClientFinderAppMutex)) {
                NotiCenterWindow.ShowWindow();
                MainWindow = new MainWindow();
                MainWindow.Show();
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                NTMinerConsole.SetIsMainUiOk(true);
            }
            else {
                Process thatProcess = null;
                Process currentProcess = Process.GetCurrentProcess();
                Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (Process process in Processes) {
                    if (process.Id != currentProcess.Id) {
                        thatProcess = process;
                        break;
                    }
                }
                if (thatProcess != null) {
                    AppUtil.Show(thatProcess);
                }
                else {
                    MessageBox.Show("另一个矿机雷达已在运行", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                Environment.Exit(0);
                return;
            }

            base.OnStartup(e);
        }
    }
}
