using System.Text;

namespace NTMiner.User {
    public class UserUpdateData : ISignableData {
        public UserUpdateData() {
        }

        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
