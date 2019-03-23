using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;
using NTMiner.Notifications;

namespace NTMiner.Vms {
    public class MinerClientAddViewModel : ViewModelBase {
        private string _leftIp = "192.168.0.100";
        private string _rightIp = "192.168.0.200";
        private bool _isIpRange;
        private string _message;
        private Visibility _messageVisible = Visibility.Collapsed;

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerClientAddViewModel() {
            this.Save = new DelegateCommand(() => {
                IPAddress leftIp;
                if (!IPAddress.TryParse(this.LeftIp, out leftIp)) {
                    this.ShowMessage("IP格式不正确");
                    return;
                }

                if (this.IsIpRange) {
                    IPAddress rightIp;
                    if (!IPAddress.TryParse(this.RightIp, out rightIp)) {
                        this.ShowMessage("IP格式不正确");
                        return;
                    }

                    uint leftValue = IpToInt(this.LeftIp);
                    uint rightValue = IpToInt(this.RightIp);
                    if (rightValue >= leftValue) {
                        List<string> clientIps = new List<string>();
                        for (uint ip = leftValue; ip <= rightValue; ip++) {
                            clientIps.Add(IntToIp(ip));
                        }

                        if (clientIps.Count > 101) {
                            this.ShowMessage("最多支持一次添加101个IP");
                            return;
                        }

                        if (clientIps.Count == 0) {
                            this.ShowMessage("没有IP");
                            return;
                        }
                        Server.ControlCenterService.AddClientsAsync(clientIps, (response, e) => {
                            if (!response.IsSuccess()) {
                                if (response != null) {
                                    this.ShowMessage(response.Description);
                                }
                            }
                            else {
                                MinerClientsWindowViewModel.Current.QueryMinerClients();
                                UIThread.Execute(() => {
                                    CloseWindow?.Invoke();
                                });
                            }
                        });
                    }
                    else {
                        this.ShowMessage("起始IP不能比终止IP大");
                    }
                }
                else {
                    Server.ControlCenterService.AddClientsAsync(new List<string> { this.LeftIp }, (response, e) => {
                        if (!response.IsSuccess()) {
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                                throw new ValidationException(response.Description);
                            }
                        }
                        else {
                            MinerClientsWindowViewModel.Current.QueryMinerClients();
                            UIThread.Execute(() => {
                                CloseWindow?.Invoke();
                            });
                        }
                    });
                }
            });
        }

        public static uint IpToInt(string ipStr) {
            string[] ip = ipStr.Split('.');
            uint ipcode = 0xFFFFFF00 | byte.Parse(ip[3]);
            ipcode = ipcode & 0xFFFF00FF | (uint.Parse(ip[2]) << 0x8);
            ipcode = ipcode & 0xFF00FFFF | (uint.Parse(ip[1]) << 0xF);
            ipcode = ipcode & 0x00FFFFFF | (uint.Parse(ip[0]) << 0x18);
            return ipcode;
        }
        public static string IntToIp(uint ipcode) {
            byte a = (byte)((ipcode & 0xFF000000) >> 0x18);
            byte b = (byte)((ipcode & 0x00FF0000) >> 0xF);
            byte c = (byte)((ipcode & 0x0000FF00) >> 0x8);
            byte d = (byte)(ipcode & 0x000000FF);
            string ipStr = string.Format("{0}.{1}.{2}.{3}", a, b, c, d);
            return ipStr;
        }

        private void ShowMessage(string message) {
            this.Message = message;
            MessageVisible = Visibility.Visible;
            TimeSpan.FromSeconds(4).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    MessageVisible = Visibility.Collapsed;
                });
            });
        }

        public string Message {
            get => _message;
            set {
                if (_message != value) {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        public Visibility MessageVisible {
            get => _messageVisible;
            set {
                if (_messageVisible != value) {
                    _messageVisible = value;
                    OnPropertyChanged(nameof(MessageVisible));
                }
            }
        }

        public string LeftIp {
            get => _leftIp;
            set {
                _leftIp = value;
                OnPropertyChanged(nameof(LeftIp));
                IPAddress ip;
                if (!IPAddress.TryParse(value, out ip)) {
                    throw new ValidationException("IP格式不正确");
                }
            }
        }

        public bool IsIpRange {
            get => _isIpRange;
            set {
                _isIpRange = value;
                OnPropertyChanged(nameof(IsIpRange));
            }
        }

        public string RightIp {
            get => _rightIp;
            set {
                _rightIp = value;
                OnPropertyChanged(nameof(RightIp));
                IPAddress ip;
                if (!IPAddress.TryParse(value, out ip)) {
                    throw new ValidationException("IP格式不正确");
                }
            }
        }
    }
}
