using System;
using System.ComponentModel;
using System.Windows;

namespace NTMiner.Vms {
    public abstract class ViewModelBase : INotifyPropertyChanged {
        public bool IsDebugMode {
            get {
                if (Design.IsInDesignMode) {
                    return true;
                }
                return DevMode.IsDebugMode;
            }
        }

        public bool IsNotDebugMode => !IsDebugMode;

        public Visibility IsDebugModeVisible {
            get {
                if (IsDebugMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsDevModeVisible {
            get {
                if (DevMode.IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void AllPropertyChanged() {
            Type type = this.GetType();
            foreach (var propertyInfo in type.GetProperties()) {
                this.OnPropertyChanged(propertyInfo.Name);
            }
        }
    }
}
