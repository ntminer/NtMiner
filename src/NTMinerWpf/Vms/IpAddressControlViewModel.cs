
namespace NTMiner.Vms {
    using System;

    public class IpAddressViewModel : ViewModelBase {
        public event EventHandler AddressChanged;

        public IpAddressViewModel(string address) {
            SetAddress(address);
        }

        public string AddressText {
            get { return $"{Part1 ?? "0"}.{Part2 ?? "0"}.{Part3 ?? "0"}.{Part4 ?? "0"}"; }
        }

        private bool _isPart1Focused;

        public bool IsPart1Focused {
            get { return _isPart1Focused; }
            set {
                _isPart1Focused = value;
                OnPropertyChanged(nameof(IsPart1Focused));
            }
        }

        private string _part1;

        public string Part1 {
            get { return _part1; }
            set {
                _part1 = value;
                SetFocus(true, false, false, false);

                var moveNext = CanMoveNext(ref _part1);

                OnPropertyChanged(nameof(Part1));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

                if (moveNext) {
                    SetFocus(false, true, false, false);
                }
            }
        }

        private bool _isPart2Focused;

        public bool IsPart2Focused {
            get { return _isPart2Focused; }
            set {
                _isPart2Focused = value;
                OnPropertyChanged(nameof(IsPart2Focused));
            }
        }


        private string _part2;

        public string Part2 {
            get { return _part2; }
            set {
                _part2 = value;
                SetFocus(false, true, false, false);

                var moveNext = CanMoveNext(ref _part2);

                OnPropertyChanged(nameof(Part2));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

                if (moveNext) {
                    SetFocus(false, false, true, false);
                }
            }
        }

        private bool _isPart3Focused;

        public bool IsPart3Focused {
            get { return _isPart3Focused; }
            set {
                _isPart3Focused = value;
                OnPropertyChanged(nameof(IsPart3Focused));
            }
        }

        private string _part3;

        public string Part3 {
            get { return _part3; }
            set {
                _part3 = value;
                SetFocus(false, false, true, false);
                var moveNext = CanMoveNext(ref _part3);

                OnPropertyChanged(nameof(Part3));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

                if (moveNext) {
                    SetFocus(false, false, false, true);
                }
            }
        }

        private bool _isPart4Focused;

        public bool IsPart4Focused {
            get { return _isPart4Focused; }
            set {
                _isPart4Focused = value;
                OnPropertyChanged(nameof(IsPart4Focused));
            }
        }

        private string _part4;

        public string Part4 {
            get { return _part4; }
            set {
                _part4 = value;
                SetFocus(false, false, false, true);
                var moveNext = CanMoveNext(ref _part4);

                OnPropertyChanged(nameof(Part4));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetAddress(string address) {
            if (string.IsNullOrWhiteSpace(address)) {
                return;
            }

            var parts = address.Split('.');
            if (parts.Length != 4) {
                return;
            }

            if (int.TryParse(parts[0], out var num0)) {
                Part1 = num0.ToString();
            }

            if (int.TryParse(parts[1], out var num1)) {
                Part2 = parts[1];
            }

            if (int.TryParse(parts[2], out var num2)) {
                Part3 = parts[2];
            }

            if (int.TryParse(parts[3], out var num3)) {
                Part4 = parts[3];
            }
        }

        private bool CanMoveNext(ref string part) {
            bool moveNext = false;

            if (!string.IsNullOrWhiteSpace(part)) {
                if (part.Length >= 3) {
                    moveNext = true;
                }

                if (part.EndsWith(".")) {
                    moveNext = true;
                    part = part.Replace(".", "");
                }
            }

            return moveNext;
        }

        private void SetFocus(bool part1, bool part2, bool part3, bool part4) {
            IsPart1Focused = part1;
            IsPart2Focused = part2;
            IsPart3Focused = part3;
            IsPart4Focused = part4;
        }
    }
}
