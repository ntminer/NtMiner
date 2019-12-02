using System;
using System.ComponentModel;

namespace NTMiner.Vms {
    public abstract class ViewModelBase : INotifyPropertyChanged {
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
