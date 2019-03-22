using System;
using System.ComponentModel;

namespace NTMiner.Vms {
    public class EntityViewModelBase<T, TId> : IEntity<TId> where T : IEntity<TId> {
        protected TId _id;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void AllPropertyChanged() {
            Type type = this.GetType();
            foreach (var propertyInfo in type.GetProperties(System.Reflection.BindingFlags.Public)) {
                this.OnPropertyChanged(propertyInfo.Name);
            }
        }

        public override int GetHashCode() {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (!(obj is EntityViewModelBase<T, TId> vm)) {
                return false;
            }
            return vm.Id.Equals(this.Id);
        }

        public TId GetId() {
            return this.Id;
        }

        public TId Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }
}
