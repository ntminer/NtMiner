using AxMSTSCLib;
using NTMiner.RemoteDesktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NTMiner {
    public static class MsRdpRemoteDesktop {
        private static readonly Dictionary<string, Form> SFormsByName = new Dictionary<string, Form>();
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
            string id = serverIp.Replace(".", "");
            string formName = $"form_{id}";
            string formText = $"开源矿工远程桌面 - {description} ({serverIp})";
            AxMsRdpClient7NotSafeForScripting axMsRdpc = null;
            string axMsRdpcName = $"axMsRdpc_{id}";
            if (SFormsByName.ContainsKey(formName)) {
                Form form = SFormsByName[formName];
                form.Show();
                if (form.WindowState == FormWindowState.Minimized) {
                    form.WindowState = FormWindowState.Normal;
                }
                form.Activate();
                return;
            }
            else {
                axMsRdpc = new AxMsRdpClient7NotSafeForScripting();
            }
            Form axMsRdpcForm = new Form {
                ShowIcon = false,
                Name = formName,
                Text = formText,
                Size = new Size(1200, 900),
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                WindowState = FormWindowState.Normal
            };
            axMsRdpcForm.FormClosed += (object sender, FormClosedEventArgs e) => {
                Form frm = (Form)sender;
                foreach (Control ctrl in frm.Controls) {
                    if (ctrl.GetType().Name == nameof(AxMsRdpClient7NotSafeForScripting)) {
                        if (SFormsByName.ContainsKey(frm.Name)) {
                            SFormsByName.Remove(frm.Name);
                        }
                        var axMsRdp = (AxMsRdpClient7NotSafeForScripting)ctrl;
                        if (axMsRdp != null && axMsRdp.Connected != 0) {
                            axMsRdp.Disconnect();
                            axMsRdp.Dispose();
                        }
                    }
                }
            };
            SFormsByName.Add(formName, axMsRdpcForm);

            ((System.ComponentModel.ISupportInitialize)(axMsRdpc)).BeginInit();
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

            axMsRdpcForm.Controls.Add(axMsRdpc);
            axMsRdpcForm.Show();
            ((System.ComponentModel.ISupportInitialize)(axMsRdpc)).EndInit();

            axMsRdpc.Name = axMsRdpcName;
            axMsRdpc.Server = serverIp;
            axMsRdpc.UserName = userName;
            axMsRdpc.AdvancedSettings7.RDPPort = serverIps.Length == 1 ? 3389 : Convert.ToInt32(serverIps[1]);
            // 启用CredSSP身份验证（有些服务器连接没有反应，需要开启这个）
            axMsRdpc.AdvancedSettings7.EnableCredSspSupport = true;
            axMsRdpc.AdvancedSettings7.ClearTextPassword = password;
            // 禁用公共模式
            //axMsRdpc.AdvancedSettings7.PublicMode = false;
            // 颜色位数 8,16,24,32
            axMsRdpc.ColorDepth = 32;
            axMsRdpc.FullScreen = false;
            axMsRdpc.DesktopWidth = axMsRdpcForm.ClientRectangle.Width;
            axMsRdpc.DesktopHeight = axMsRdpcForm.ClientRectangle.Height;
            axMsRdpc.Connect();
        }
    }
}
