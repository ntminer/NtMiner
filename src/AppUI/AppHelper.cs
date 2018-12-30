using NTMiner.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public static class AppHelper {
        public static void RunAsAdministrator() {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ClientId.AppFileFullName;
            startInfo.Arguments = string.Join(" ", CommandLineArgs.Args);
            startInfo.Verb = "runas";
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        public static SolidColorBrush GetSolidColor(string key) {
            return (SolidColorBrush)Application.Current.Resources[key];
        }

        #region Init
        public static void Init(Application app) {
            BootLog.SetLogDir(Path.Combine(Global.GlobalDirFullName, "Logs"));
            BootLog.Log("AppHelper.Init start");
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                var exception = e.ExceptionObject as Exception;
                if (exception != null) {
                    Handle(exception);
                }
            };

            app.DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => {
                Handle(e.Exception);
                e.Handled = true;
            };

            Execute.InitializeWithDispatcher();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            BootLog.Log("AppHelper.Init end");
        }
        #endregion

        #region MainWindowShowed
        public static void MainWindowShowed() {
            try {
                var resultPipeClient = new NamedPipeClientStream(".", "resultPipName", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                resultPipeClient.Connect(200);
                StreamWriter sw = new StreamWriter(resultPipeClient);
                sw.WriteLine("MainWindowShowed");
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex) {
                Global.Logger.Error(ex.Message, ex);
            }
        }
        #endregion

        #region RunPipeServer
        public static void RunPipeServer(Application app, string appPipName) {
            while (true) {
                try {
                    NamedPipeServerStream pipeServer = new NamedPipeServerStream(appPipName, PipeDirection.InOut, 2);
                    pipeServer.WaitForConnection();
                    StreamReader sr = new StreamReader(pipeServer);
                    string recData = sr.ReadLine();
                    if (recData == "ShowMainWindow") {
                        Global.Execute(new ShowMainWindowCommand());
                    }
                    else if (recData == "CloseMainWindow") {
                        Execute.OnUIThread(() => {
                            app.MainWindow.Close();
                        });
                    }
                    sr.Close();
                }
                catch (Exception ex) {
                    Global.Logger.Error(ex.Message, ex);
                }
            }
        }
        #endregion

        #region ShowMainWindow
        public static void ShowMainWindow(Application app, string appPipName) {
            Task.Factory.StartNew(() => {
                RunResultPipServer(app);
            });
            try {
                var pipeClient = new NamedPipeClientStream(".", appPipName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
                pipeClient.Connect(200);
                StreamWriter sw = new StreamWriter(pipeClient);
                sw.WriteLine("ShowMainWindow");
                sw.Flush();
                Thread.Sleep(1000);
                sw.Close();
            }
            catch (Exception ex) {
                NTMinerClientDaemon.Instance.RestartNTMiner(Global.Localhost, Global.ClientPort, CommandLineArgs.WorkId, null);
                Global.Logger.Error(ex.Message, ex);
            }
            TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                Process thisProcess = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(thisProcess.ProcessName)) {
                    if (process.Id != thisProcess.Id) {
                        try {
                            Windows.TaskKill.Kill(process.Id);
                        }
                        catch {
                        }
                    }
                }
                Windows.Cmd.RunClose(ClientId.AppFileFullName, string.Join(" ", CommandLineArgs.Args));
                Execute.OnUIThread(() => {
                    app.Shutdown();
                });
            });
        }
        #endregion

        #region private methods
        private static void Handle(Exception e) {
            if (e == null) {
                return;
            }
            if (e is ValidationException) {
                DialogWindow.ShowDialog(title: "验证失败", message: e.Message, icon: "Icon_Error");
            }
            else {
                Global.Logger.Error(e);
            }
        }

        private static void RunResultPipServer(Application app) {
            while (true) {
                try {
                    NamedPipeServerStream pipeServer = new NamedPipeServerStream("resultPipName", PipeDirection.InOut, 2);
                    pipeServer.WaitForConnection();
                    StreamReader sr = new StreamReader(pipeServer);
                    string recData = sr.ReadLine();
                    if (recData == "MainWindowShowed") {
                        Execute.OnUIThread(() => {
                            app.Shutdown();
                        });
                    }
                    sr.Close();
                }
                catch (Exception ex) {
                    Global.Logger.Error(ex.Message, ex);
                }
            }
        }
        #endregion
    }
}
