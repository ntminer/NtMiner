using System;

namespace NTMiner.User {
    public class SignUpRequest : ISignUpInput, IRequest {
        public SignUpRequest() { }

        public static SignUpRequest Create(ISignUpInput data) {
            return new SignUpRequest {
                ActionCaptcha = data.ActionCaptcha,
                ActionCaptchaId = data.ActionCaptchaId,
                Email = data.Email,
                EmailCode = data.EmailCode,
                LoginName = data.LoginName,
                Mobile = data.Mobile,
                MobileCode = data.MobileCode,
                Password = data.Password,
                PasswordAgain = data.PasswordAgain
            };
        }

        public UserData ToUserData(string publicKey, string privateKey) {
            return new UserData {
                LoginName = this.LoginName,
                IsEnabled = true,
                CreatedOn = DateTime.Now,
                Email = this.Email,
                Mobile = this.Mobile,
                Password = this.Password,
                Description = string.Empty,
                Roles = string.Empty,
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }

        public string LoginName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Password { get; set; }

        public string PasswordAgain { get; set; }
        public Guid ActionCaptchaId { get; set; }
        /// <summary>
        /// <see cref="ISignUpInput.ActionCaptcha"/>
        /// </summary>
        public string ActionCaptcha { get; set; }
        /// <summary>
        /// <see cref="ISignUpInput.EmailCode"/>
        /// </summary>
        public string EmailCode { get; set; }
        /// <summary>
        /// <see cref="ISignUpInput.MobileCode"/>
        /// </summary>
        public string MobileCode { get; set; }
    }
}
