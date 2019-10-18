
namespace NTMiner.Controls {
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class IPRangeValidationRule : ValidationRule {
        private int _min;
        private int _max;

        public int Min {
            get { return _min; }
            set { _min = value; }
        }

        public int Max {
            get { return _max; }
            set { _max = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            int val = 0;
            var strVal = (string)value;
            try {
                if (strVal.Length > 0) {
                    if (strVal.EndsWith(".")) {
                        return CheckRanges(strVal.Replace(".", ""));
                    }

                    // Allow dot character to move to next box
                    return CheckRanges(strVal);
                }
            }
            catch (Exception e) {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if ((val < Min) || (val > Max)) {
                return new ValidationResult(false,
                  "Please enter the value in the range: " + Min + " - " + Max + ".");
            }
            else {
                return ValidationResult.ValidResult;
            }
        }

        private ValidationResult CheckRanges(string strVal) {
            if (int.TryParse(strVal, out var res)) {
                if ((res < Min) || (res > Max)) {
                    return new ValidationResult(false,
                      "Please enter the value in the range: " + Min + " - " + Max + ".");
                }
                else {
                    return ValidationResult.ValidResult;
                }
            }
            else {
                return new ValidationResult(false, "Illegal characters entered");
            }
        }
    }
}
