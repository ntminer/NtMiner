using NTMiner.Core;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class MessageTypeItem<T> : ViewModelBase where T : struct {
        private int _count;
        private readonly Action OnIsCheckedChanged;
        private readonly string _messageTypeName;
        public MessageTypeItem(EnumItem<T> messageType, Func<T, StreamGeometry> getIcon, Func<T, SolidColorBrush> getIconFill, Action onIsCheckedChanged) {
            _messageTypeName = typeof(T).Name;
            this.MessageType = messageType;
            this.OnIsCheckedChanged = onIsCheckedChanged;
            this.Icon = getIcon(messageType.Value);
            this.IconFill = getIconFill(messageType.Value);
        }
        public EnumItem<T> MessageType { get; private set; }
        public StreamGeometry Icon { get; private set; }
        public SolidColorBrush IconFill { get; private set; }
        public string DisplayText {
            get {
                return MessageType.Description;
            }
        }

        public bool IsChecked {
            get {
                bool value = true;
                if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting($"Is{_messageTypeName}{MessageType.Name}Checked", out IAppSetting setting) && setting.Value != null) {
                    value = (bool)setting.Value;
                }
                return value;
            }
            set {
                AppSettingData appSettingData = new AppSettingData() {
                    Key = $"Is{_messageTypeName}{MessageType.Name}Checked",
                    Value = value
                };
                VirtualRoot.Execute(new SetLocalAppSettingCommand(appSettingData));
                OnPropertyChanged(nameof(IsChecked));
                OnIsCheckedChanged?.Invoke();
            }
        }

        public int Count {
            get => _count;
            set {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
    }
}
