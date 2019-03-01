using LiteDB;

namespace NTMiner {
    public class UserData : IUser {
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

        public ObjectId Id { get; set; }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public bool IsEnabled { get; set; }

        public string Description { get; set; }
    }
}
