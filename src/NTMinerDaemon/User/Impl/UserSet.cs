using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner.User.Impl {
    public class UserSet : IUserSet {
        private readonly Dictionary<string, UserData> _dicByLoginName = new Dictionary<string, UserData>();

        public UserSet() {
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    List<UserData> users = GetUsers();
                    foreach (var user in users) {
                        if (!_dicByLoginName.ContainsKey(user.LoginName)) {
                            _dicByLoginName.Add(user.LoginName, user);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        /// <summary>
        /// 同步方法
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private static List<UserData> GetUsers() {
            try {
                DataRequest<Guid?> request = new DataRequest<Guid?> {
                    Data = NTMinerRegistry.GetClientId()
                };
                using (HttpClient client = new HttpClient()) {
                    client.Timeout = TimeSpan.FromMilliseconds(2000);
                    Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{NTMinerRegistry.GetControlCenterHost()}:{NTKeyword.ControlCenterPort.ToString()}/api/ControlCenter/Users", request);
                    DataResponse<List<UserData>> response = getHttpResponse.Result.Content.ReadAsAsync<DataResponse<List<UserData>>>().Result;
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return new List<UserData>();
        }

        public void Refresh() {
            _dicByLoginName.Clear();
            _isInited = false;
        }

        public IUser GetUser(string loginName) {
            InitOnece();
            if (_dicByLoginName.TryGetValue(loginName, out UserData userData)) {
                return userData;
            }
            return null;
        }

        public IEnumerable<IUser> AsEnumerable() {
            InitOnece();
            return _dicByLoginName.Values;
        }
    }
}
