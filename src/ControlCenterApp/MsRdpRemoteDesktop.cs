using AxMSTSCLib;
using NTMiner.RemoteDesktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NTMiner {
    public class MsRdpRemoteDesktop : IRemoteDesktop {
        private static readonly Dictionary<string, Form> axMsRdpcArray = new Dictionary<string, Form>();
        /// <summary>
        /// 创建远程桌面连接
        /// </summary>
        /// <param name="serverIp">serverIp可带端口或不带端口，不带短则则使用远程桌面默认的3389</param>
        public void OpenRemoteDesktop(string serverIp, string userName, string password, string description) {
            string[] serverIps = serverIp.Split(':');
            serverIp = serverIps[0];
            Form axMsRdpcForm = new Form();
            axMsRdpcForm.ShowIcon = false;
            string id = serverIp.Replace(".", "");
            string formName = $"form_{id}";
            string formText = $"{description} ({serverIp})";
            AxMsRdpClient7NotSafeForScripting axMsRdpc = null;
            // 给axMsRdpc取个名字
            string axMsRdpcName = $"axMsRdpc_{id}";
            if (axMsRdpcArray.ContainsKey(formName)) {
                Form form = axMsRdpcArray[formName];
                form.Show();
                form.Activate();
                return;
            }
            else {
                axMsRdpc = new AxMsRdpClient7NotSafeForScripting();
            }
            axMsRdpcForm.Name = formName;
            axMsRdpcForm.Text = formText;
            axMsRdpcForm.Size = new Size(1200, 900);
            axMsRdpcForm.StartPosition = FormStartPosition.CenterScreen;
            axMsRdpcForm.MaximizeBox = false;
            axMsRdpcForm.WindowState = FormWindowState.Normal;
            axMsRdpcForm.FormClosed += (object sender, FormClosedEventArgs e) => {
                Form frm = (Form)sender;
                foreach (Control ctrl in frm.Controls) {
                    // 找到当前打开窗口下面的远程桌面
                    if (ctrl.GetType().Name == nameof(AxMsRdpClient7NotSafeForScripting)) {
                        // 释放缓存
                        if (axMsRdpcArray.ContainsKey(frm.Name)) {
                            axMsRdpcArray.Remove(frm.Name);
                        }
                        // 断开连接
                        var _axMsRdp = ctrl as AxMsRdpClient7NotSafeForScripting;
                        if (_axMsRdp.Connected != 0) {
                            _axMsRdp.Disconnect();
                            _axMsRdp.Dispose();
                        }
                    }
                }
            };
            // 添加到当前缓存
            axMsRdpcArray.Add(formName, axMsRdpcForm);

            ((System.ComponentModel.ISupportInitialize)(axMsRdpc)).BeginInit();
            axMsRdpc.Dock = DockStyle.Fill;
            axMsRdpc.Enabled = true;

            // 绑定连接与释放事件
            axMsRdpc.OnConnecting += (object sender, EventArgs e) => {
                var _axMsRdp = sender as AxMsRdpClient7NotSafeForScripting;
                _axMsRdp.ConnectingText = _axMsRdp.GetStatusText(Convert.ToUInt32(_axMsRdp.Connected));
                _axMsRdp.FindForm().WindowState = FormWindowState.Normal;
            };
            axMsRdpc.OnDisconnected += (object sender, IMsTscAxEvents_OnDisconnectedEvent e) => {
                var _axMsRdp = sender as AxMsRdpClient7NotSafeForScripting;
                string disconnectedText = $"远程桌面 {formText} 连接已断开！";
                _axMsRdp.DisconnectedText = disconnectedText;
                _axMsRdp.FindForm().Close();
                MessageBox.Show(disconnectedText, "远程连接");
            };

            axMsRdpcForm.Controls.Add(axMsRdpc);
            axMsRdpcForm.Show();
            ((System.ComponentModel.ISupportInitialize)(axMsRdpc)).EndInit();

            // RDP名字
            axMsRdpc.Name = axMsRdpcName;
            // 服务器地址
            axMsRdpc.Server = serverIp;
            // 远程登录账号
            axMsRdpc.UserName = userName;
            // 远程端口号
            axMsRdpc.AdvancedSettings7.RDPPort = serverIps.Length == 1 ? 3389 : Convert.ToInt32(serverIps[1]);
            // 启用CredSSP身份验证（有些服务器连接没有反应，需要开启这个）
            axMsRdpc.AdvancedSettings7.EnableCredSspSupport = true;
            // 远程登录密码
            axMsRdpc.AdvancedSettings7.ClearTextPassword = password;
            // 禁用公共模式
            //axMsRdpc.AdvancedSettings7.PublicMode = false;
            // 颜色位数 8,16,24,32
            axMsRdpc.ColorDepth = 32;
            // 开启全屏 true|flase
            axMsRdpc.FullScreen = false;
            // 设置远程桌面宽度为显示器宽度
            //axMsRdpc.DesktopWidth = ScreenArea.Width;
            axMsRdpc.DesktopWidth = axMsRdpcForm.ClientRectangle.Width;
            // 设置远程桌面宽度为显示器高度
            //axMsRdpc.DesktopHeight = ScreenArea.Height;
            axMsRdpc.DesktopHeight = axMsRdpcForm.ClientRectangle.Height;
            // 远程连接
            axMsRdpc.Connect();
        }
    }
}
