using System;
using System.Text;

namespace NTMiner.User {
    /// <summary>
    /// <see cref="ILoginedUser"/>
    /// </summary>
    public class LoginedUser : ILoginedUser, ISignableData {
        public static readonly LoginedUser Empty = new LoginedUser {
            CreatedOn = DateTime.MinValue,
            Description = string.Empty,
            Email = string.Empty,
            IsEnabled = true,
            LoginName = string.Empty,
            Mobile = string.Empty,
            Roles = string.Empty,
            PublicKey = string.Empty
        };

        public LoginedUser() {
        }

        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public bool IsEnabled { get; set; }

        public string PublicKey { get; set; }

        public string Roles { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
