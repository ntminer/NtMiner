
namespace NTMiner.Vms {
    using System;

    public class IpAddressViewModel : ViewModelBase {
        public IpAddressViewModel(string address) {
            SetAddress(address);
        }

        public string AddressText {
            get { return $"{(string.IsNullOrEmpty(Part1) ? "0" : Part1)}.{(string.IsNullOrEmpty(Part2) ? "0" : Part2)}.{(string.IsNullOrEmpty(Part3) ? "0" : Part3)}.{(string.IsNullOrEmpty(Part4) ? "0" : Part4)}"; }
        }

        public bool IsAnyEmpty {
            get {
                return string.IsNullOrEmpty(Part1) || string.IsNullOrEmpty(Part2) || string.IsNullOrEmpty(Part3) || string.IsNullOrEmpty(Part4);
            }
        }

        private string _currentFocused;

        public bool IsPart1Focused {
            get { return _currentFocused == nameof(IsPart1Focused); }
            set {
                SetFocus(nameof(IsPart1Focused));
            }
        }

        private string _part1;

        public string Part1 {
            get { return _part1; }
            set {
                _part1 = value;
                SetFocus(nameof(IsPart1Focused));

                var moveNext = CanMoveNext(ref _part1);

                OnPropertyChanged(nameof(Part1));
                OnPropertyChanged(nameof(AddressText));

                if (moveNext) {
                    SetFocus(nameof(IsPart2Focused));
                }
            }
        }

        public bool IsPart2Focused {
            get { return _currentFocused == nameof(IsPart2Focused); }
            set {
                SetFocus(nameof(IsPart2Focused));
            }
        }


        private string _part2;

        public string Part2 {
            get { return _part2; }
            set {
                _part2 = value;
                SetFocus(nameof(IsPart2Focused));

                var moveNext = CanMoveNext(ref _part2);

                OnPropertyChanged(nameof(Part2));
                OnPropertyChanged(nameof(AddressText));

                if (moveNext) {
                    SetFocus(nameof(IsPart3Focused));
                }
            }
        }

        public bool IsPart3Focused {
            get { return _currentFocused == nameof(IsPart3Focused); }
            set {
                SetFocus(nameof(IsPart3Focused));
            }
        }

        private string _part3;

        public string Part3 {
            get { return _part3; }
            set {
                _part3 = value;
                SetFocus(nameof(IsPart3Focused));
                var moveNext = CanMoveNext(ref _part3);

                OnPropertyChanged(nameof(Part3));
                OnPropertyChanged(nameof(AddressText));

                if (moveNext) {
                    SetFocus(nameof(IsPart4Focused));
                }
            }
        }

        public bool IsPart4Focused {
            get { return _currentFocused == nameof(IsPart4Focused); }
            set {
                SetFocus(nameof(IsPart4Focused));
            }
        }

        private string _part4;

        public string Part4 {
            get { return _part4; }
            set {
                _part4 = value;
                SetFocus(nameof(IsPart4Focused));
                var moveNext = CanMoveNext(ref _part4);

                OnPropertyChanged(nameof(Part4));
                OnPropertyChanged(nameof(AddressText));
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

            if (int.TryParse(parts[0], out var _)) {
                _part1 = parts[0];
            }

            if (int.TryParse(parts[1], out var _)) {
                _part2 = parts[1];
            }

            if (int.TryParse(parts[2], out var _)) {
                _part3 = parts[2];
            }

            if (int.TryParse(parts[3], out var _)) {
                _part4 = parts[3];
            }
            OnPropertyChanged(nameof(Part1));
            OnPropertyChanged(nameof(Part2));
            OnPropertyChanged(nameof(Part3));
            OnPropertyChanged(nameof(Part4));
            OnPropertyChanged(nameof(AddressText));
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

        private void SetFocus(string focus) {
            _currentFocused = focus;
            OnPropertyChanged(nameof(IsPart1Focused));
            OnPropertyChanged(nameof(IsPart2Focused));
            OnPropertyChanged(nameof(IsPart3Focused));
            OnPropertyChanged(nameof(IsPart4Focused));
        }
    }
}
