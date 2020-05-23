using System.Linq;

namespace NTMiner.User {
    /// <summary>
    /// 因为该类型不能序列化属于内部的类型，所以不放在NTMinerDataSchemas程序集里
    /// </summary>
    public class UserId {
        public static readonly UserId Empty = new UserId() { 
            UserIdType = UserIdType.LoginName,
            Value = string.Empty
        };

        public static UserId CreateLoginNameUserId(string loginName) {
            return new UserId {
                UserIdType = UserIdType.LoginName,
                Value = loginName
            };
        }

        public static UserId Create(string userId) {
            UserId result = new UserId {
                Value = userId,
                UserIdType = UserIdType.LoginName
            };
            if (!string.IsNullOrEmpty(userId)) {
                /// 若包含@符号则视为邮箱，所以需要确保注册时的LoginName中不能包含@符号 <see cref="VirtualRoot.IsValidLoginName"/>
                if (userId.IndexOf('@') != -1) {
                    result.UserIdType = UserIdType.Email;
                }
                /// 若是11位纯数字则视为手机号码，所以需确保注册时的LoginName不能是11位纯数字 <see cref="VirtualRoot.IsValidLoginName"/>
                else if (userId.Length == 11 && userId.All(a => char.IsDigit(a))) {
                    result.UserIdType = UserIdType.Mobile;
                }
            }
            return result;
        }

        private UserId() { }

        public UserIdType UserIdType { get; private set; }
        public string Value { get; private set; }
    }
}
