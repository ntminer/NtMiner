using System.Text;

namespace NTMiner.User {
    public class QueryUsersRequest : IRequest, ISignableData {
        private int _pageIndex;
        private int _pageSize;

        public QueryUsersRequest() { }
        public int PageIndex {
            get => _pageIndex;
            set {
                if (value < 0) {
                    value = 0;
                }
                _pageIndex = value;
            }
        }
        public int PageSize {
            get => _pageSize;
            set {
                if (value < 10) {
                    value = 10;
                }
                if (value > 1000) {
                    value = 1000;
                }
                _pageSize = value;
            }
        }

        public string LoginName { get; set; }
        public UserStatus UserStatus { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
