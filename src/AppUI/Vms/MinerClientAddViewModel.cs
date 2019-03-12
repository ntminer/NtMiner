using System;
using System.Net;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientAddViewModel : ViewModelBase {
        private string _leftIp = "192.168.0.1";
        private string _rightIp = "192.168.0.255";
        private bool _isIpRange;

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerClientAddViewModel() {
            this.Save = new DelegateCommand(() => {
                IPAddress leftIp;
                if (!IPAddress.TryParse(this.LeftIp, out leftIp)) {
                    throw new ValidationException("IP格式不正确");
                }

                if (this.IsIpRange) {
                    IPAddress rightIp;
                    if (!IPAddress.TryParse(this.RightIp, out rightIp)) {
                        throw new ValidationException("IP格式不正确");
                    }

                    uint leftValue = IpToInt(this.LeftIp);
                    uint rightValue = IpToInt(this.RightIp);
                    if (rightValue >= leftValue) {
                        for (uint ip = leftValue; ip <= rightValue; ip++) {
                            Server.ControlCenterService.AddClientAsync(IntToIp(ip), (response, e) => {
                            });
                        }
                    }
                    else {
                        throw new ValidationException("天啊，起始ip居然比终止ip还大");
                    }
                    UIThread.Execute(() => {
                        CloseWindow?.Invoke();
                    });
                }
                else {
                    Server.ControlCenterService.AddClientAsync(this.LeftIp, (response, e) => {
                        if (!response.IsSuccess()) {
                            if (response != null) {
                                Write.UserLine(response.Description, ConsoleColor.Red);
                                throw new ValidationException(response.Description);
                            }
                        }
                        else {
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
