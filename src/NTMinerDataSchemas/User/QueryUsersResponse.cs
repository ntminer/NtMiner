using System.Collections.Generic;

namespace NTMiner.User {
    public class QueryUsersResponse : ResponseBase {
        public QueryUsersResponse() {
            this.Data = new List<UserData>();
        }

        public static QueryUsersResponse Ok(List<UserData> data, int total) {
            return new QueryUsersResponse() {
                StateCode = 200,
                ReasonPhrase = "Ok",
                Description = "成功",
                Data = data,
                Total = total
            };
        }

        public List<UserData> Data { get; set; }

        public int Total { get; set; }
    }
}
