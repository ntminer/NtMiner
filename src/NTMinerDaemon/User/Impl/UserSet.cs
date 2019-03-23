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
                    LoginName = string.Empty,
                    Data = NTMinerRegistry.GetClientId()
                };
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{NTMinerRegistry.GetControlCenterHost()}:{WebApiConst.ControlCenterPort}/api/ControlCenter/Users", request);
                    DataResponse<List<UserData>> response = message.Result.Content.ReadAsAsync<DataResponse<List<UserData>>>().Result;
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                }
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
            }
            return new List<UserData>();
        }

        public void Refresh() {
            _dicByLoginName.Clear();
            _isInited = false;
        }

        public IUser GetUser(string loginName) {
            InitOnece();
            UserData userData;
            if (_dicByLoginName.TryGetValue(loginName, out userData)) {
                return userData;
            }
            return null;
        }

        public IEnumerator<IUser> GetEnumerator() {
            InitOnece();
            return _dicByLoginName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicByLoginName.Values.GetEnumerator();
        }
    }
}
