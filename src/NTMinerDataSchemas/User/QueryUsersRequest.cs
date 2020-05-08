using System.Text;

namespace NTMiner.User {
    public class QueryUsersRequest : IPagingRequest, ISignableData {
        public QueryUsersRequest() { }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string LoginName { get; set; }
        public UserStatus UserStatus { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("FDDB8C9E-86A5-4F26-BCE3-FB8C61C9E0AC");
        }
    }
}
