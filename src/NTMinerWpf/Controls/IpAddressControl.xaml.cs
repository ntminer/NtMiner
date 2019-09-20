using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Controls {
    public partial class IpAddressControl : UserControl {
        public IpAddressControl() {
            InitializeComponent();
            DataObject.AddPastingHandler(part1, TextBox_Pasting);

        }


        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e) {
            var vm = this.DataContext as IpAddressViewModel;
            if (e.DataObject.GetDataPresent(typeof(String))) {
                String pastingText = (String)e.DataObject.GetData(typeof(String));
                vm.SetAddress(pastingText);
                part1.Focus();
                e.CancelCommand();
            }

        }

        private void Part4_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Back && part4.Text == "") {
                part3.CaretIndex = part3.Text.Length;
                part3.Focus();
            }
            if (e.Key == Key.Left && part4.CaretIndex == 0) {
                part3.Focus();
                e.Handled = true;
            }

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.C) {
                if (part4.SelectionLength == 0) {
                    var vm = this.DataContext as IpAddressViewModel;
                    Clipboard.SetText(vm.AddressText);
                }
            }
        }

        private void Part2_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Back && part2.Text == "") {
                part1.CaretIndex = part1.Text.Length;
                part1.Focus();
            }
            if (e.Key == Key.Right && part2.CaretIndex == part2.Text.Length) {
                part3.Focus();
                e.Handled = true;
            }
            if (e.Key == Key.Left && part2.CaretIndex == 0) {
                part1.Focus();
                e.Handled = true;
            }

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.C) {
                if (part2.SelectionLength == 0) {
                    var vm = this.DataContext as IpAddressViewModel;
                    Clipboard.SetText(vm.AddressText);
                }
            }
        }

        private void Part3_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Back && part3.Text == "") {
                part2.CaretIndex = part2.Text.Length;
                part2.Focus();
            }
            if (e.Key == Key.Right && part3.CaretIndex == part3.Text.Length) {
                part4.Focus();
                e.Handled = true;
            }
            if (e.Key == Key.Left && part3.CaretIndex == 0) {
                part2.Focus();
                e.Handled = true;
            }

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.C) {
                if (part3.SelectionLength == 0) {
                    var vm = this.DataContext as IpAddressViewModel;
                    Clipboard.SetText(vm.AddressText);
                }
            }
        }

        private void Part1_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Right && part1.CaretIndex == part1.Text.Length) {
                part2.Focus();
                e.Handled = true;
            }
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.C) {
                if (part1.SelectionLength == 0) {
                    var vm = this.DataContext as IpAddressViewModel;
                    Clipboard.SetText(vm.AddressText);
                }
            }
        }
    }
}
