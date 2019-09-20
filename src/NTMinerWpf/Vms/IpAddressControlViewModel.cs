
namespace NTMiner.Vms {
    using System;

    public class IpAddressViewModel : ViewModelBase {
        public event EventHandler AddressChanged;

        public IpAddressViewModel() { }

        public string AddressText {
            get { return $"{Part1 ?? "0"}.{Part2 ?? "0"}.{Part3 ?? "0"}.{Part4 ?? "0"}"; }
        }

        private bool isPart1Focused;

        public bool IsPart1Focused {
            get { return isPart1Focused; }
            set {
                isPart1Focused = value;
                OnPropertyChanged(nameof(IsPart1Focused));
            }
        }

        private string part1;

        public string Part1 {
            get { return part1; }
            set {
                part1 = value;
                SetFocus(true, false, false, false);

                var moveNext = CanMoveNext(ref part1);

                OnPropertyChanged(nameof(Part1));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

                if (moveNext) {
                    SetFocus(false, true, false, false);
                }
            }
        }

        private bool isPart2Focused;

        public bool IsPart2Focused {
            get { return isPart2Focused; }
            set {
                isPart2Focused = value;
                OnPropertyChanged(nameof(IsPart2Focused));
            }
        }


        private string part2;

        public string Part2 {
            get { return part2; }
            set {
                part2 = value;
                SetFocus(false, true, false, false);

                var moveNext = CanMoveNext(ref part2);

                OnPropertyChanged(nameof(Part2));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

                if (moveNext) {
                    SetFocus(false, false, true, false);
                }
            }
        }

        private bool isPart3Focused;

        public bool IsPart3Focused {
            get { return isPart3Focused; }
            set {
                isPart3Focused = value;
                OnPropertyChanged(nameof(IsPart3Focused));
            }
        }

        private string part3;

        public string Part3 {
            get { return part3; }
            set {
                part3 = value;
                SetFocus(false, false, true, false);
                var moveNext = CanMoveNext(ref part3);

                OnPropertyChanged(nameof(Part3));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

                if (moveNext) {
                    SetFocus(false, false, false, true);
                }
            }
        }

        private bool isPart4Focused;

        public bool IsPart4Focused {
            get { return isPart4Focused; }
            set {
                isPart4Focused = value;
                OnPropertyChanged(nameof(IsPart4Focused));
            }
        }

        private string part4;

        public string Part4 {
            get { return part4; }
            set {
                part4 = value;
                SetFocus(false, false, false, true);
                var moveNext = CanMoveNext(ref part4);

                OnPropertyChanged(nameof(Part4));
                OnPropertyChanged(nameof(AddressText));
                AddressChanged?.Invoke(this, EventArgs.Empty);

            }
        }

        public void SetAddress(string address) {
            if (string.IsNullOrWhiteSpace(address))
                return;

            var parts = address.Split('.');

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
