using System.Text;

namespace NTMiner {
    public class UserData : IUser, IGetSignData {
        public UserData() { }

        public UserData(IUser data) {
            this.LoginName = data.LoginName;
            this.Password = data.Password;
            this.IsEnabled = data.IsEnabled;
            this.Description = data.Description;
        }

        public void Update(IUser data) {
            this.LoginName = data.LoginName;
            this.Password = data.Password;
            this.IsEnabled = data.IsEnabled;
            this.Description = data.Description;
        }

        [LiteDB.BsonId]
        public string LoginName { get; set; }

        public string Password { get; set; }

        public bool IsEnabled { get; set; }

        public string Description { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LoginName)).Append(LoginName)
                .Append(nameof(Password)).Append(Password)
                .Append(nameof(IsEnabled)).Append(IsEnabled)
                .Append(nameof(Description)).Append(Description);
            return sb;
        }
    }
}
