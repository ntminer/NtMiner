using System;

namespace NTMiner.User {
    public class SignUpRequest : ISignUpInput, IRequest {
        public SignUpRequest() { }

        public static SignUpRequest Create(ISignUpInput data) {
            return new SignUpRequest {
                ActionCaptcha = data.ActionCaptcha,
                ActionCaptchaId = data.ActionCaptchaId,
                LoginName = data.LoginName,
                Password = data.Password,
                PasswordAgain = data.PasswordAgain
            };
        }

        public UserData ToUserData(string publicKey, string privateKey) {
            return new UserData {
                LoginName = this.LoginName,
                IsEnabled = true,
                CreatedOn = DateTime.Now,
                Password = this.Password,
                Description = string.Empty,
                Roles = string.Empty,
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public string PasswordAgain { get; set; }
        public Guid ActionCaptchaId { get; set; }
        /// <summary>
        /// <see cref="ISignUpInput.ActionCaptcha"/>
        /// </summary>
        public string ActionCaptcha { get; set; }
    }
}
