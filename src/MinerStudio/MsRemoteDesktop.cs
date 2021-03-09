using AxMSTSCLib;
using NTMiner.RemoteDesktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NTMiner {
    public static class MsRemoteDesktop {
        private static readonly Dictionary<string, Form> _formDicByName = new Dictionary<string, Form>();
        /// <summary>
        /// 打开远程桌面窗口连接给定ip的windows主机
        /// </summary>
        public static void OpenRemoteDesktop(RdpInput input) {
            string serverIp = input.ServerIp;
            string userName = input.UserName;
            string password = input.Password;
            string description = input.Description;
            Action<string> onDisconnected = input.OnDisconnected;
            string[] serverIps = serverIp.Split(':');
            serverIp = serverIps[0];
            int serverPort = serverIps.Length == 1 ? 3389 : Convert.ToInt32(serverIps[1]);
            string id = serverIp.Replace(".", "_");
            string formName = $"form_{id}";
            string formText = $"开源矿工远程桌面 - {description} ({serverIp})";
            if (_formDicByName.TryGetValue(formName, out Form form)) {
                form.Show();
                if (form.WindowState == FormWindowState.Minimized) {
                    form.WindowState = FormWindowState.Normal;
                }
                form.Activate();
            }
            else {
                form = new Form {
                    ShowIcon = false,
                    Name = formName,
                    Text = formText,
                    Size = new Size(1200, 900),
                    StartPosition = FormStartPosition.CenterScreen,
                    MaximizeBox = false,
                    WindowState = FormWindowState.Normal
                };
                form.FormClosed += (object sender, FormClosedEventArgs e) => {
                    Form frm = (Form)sender;
                    foreach (Control ctrl in frm.Controls) {
                        if (ctrl.GetType().Name == nameof(AxMsRdpClient7NotSafeForScripting)) {
                            _formDicByName.Remove(frm.Name);
                            var axMsRdp = (AxMsRdpClient7NotSafeForScripting)ctrl;
                            if (axMsRdp != null) {
                                if (axMsRdp.Connected != 0) {
                                    axMsRdp.Disconnect();
                                }
                                axMsRdp.Dispose();
                            }
                        }
                    }
                };
                _formDicByName.Add(formName, form);

                AxMsRdpClient7NotSafeForScripting axMsRdpc = new AxMsRdpClient7NotSafeForScripting();
                ((System.ComponentModel.ISupportInitialize)axMsRdpc).BeginInit();
                axMsRdpc.Dock = DockStyle.Fill;
                axMsRdpc.Enabled = true;

                axMsRdpc.OnConnecting += (object sender, EventArgs e) => {
                    var axMsRdp = sender as AxMsRdpClient7NotSafeForScripting;
                    axMsRdp.ConnectingText = axMsRdp.GetStatusText(Convert.ToUInt32(axMsRdp.Connected));
                    axMsRdp.FindForm().WindowState = FormWindowState.Normal;
                };
                axMsRdpc.OnDisconnected += (object sender, IMsTscAxEvents_OnDisconnectedEvent e) => {
                    var axMsRdp = sender as AxMsRdpClient7NotSafeForScripting;
                    string disconnectedText = $"{formText}远程桌面连接已断开！";
                    axMsRdp.DisconnectedText = disconnectedText;
                    axMsRdp.FindForm().Close();
                    onDisconnected?.Invoke(disconnectedText);
                };

                form.Controls.Add(axMsRdpc);
                form.Show();
                ((System.ComponentModel.ISupportInitialize)(axMsRdpc)).EndInit();

                axMsRdpc.Name = $"axMsRdpc_{id}";
                axMsRdpc.Server = serverIp;
                axMsRdpc.UserName = userName;
                axMsRdpc.AdvancedSettings7.RDPPort = serverPort;
                // 启用CredSSP身份验证（有些服务器连接没有反应，需要开启这个）
                axMsRdpc.AdvancedSettings7.EnableCredSspSupport = true;
                axMsRdpc.AdvancedSettings7.ClearTextPassword = password;
                // 禁用公共模式
                //axMsRdpc.AdvancedSettings7.PublicMode = false;
                // 颜色位数 8,16,24,32
                axMsRdpc.ColorDepth = 32;
                axMsRdpc.FullScreen = false;
                axMsRdpc.DesktopWidth = form.ClientRectangle.Width;
                axMsRdpc.DesktopHeight = form.ClientRectangle.Height;
                axMsRdpc.Connect();
            }
        }
    }
}
