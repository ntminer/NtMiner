using NTMiner.User;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using static NTMiner.Controllers.ApiControllerBase;

namespace NTMiner.Role {
    public class UserAttribute : ActionFilterAttribute {
        private static readonly MethodInfo _methodInfo = typeof(UserAttribute).GetMethod(nameof(IsValidUser), BindingFlags.NonPublic | BindingFlags.Static);

        protected virtual bool OnAuthorization(UserData user, out string message) {
            message = string.Empty;
            return true;
        }

        public override void OnActionExecuting(HttpActionContext actionContext) {
            base.OnActionExecuting(actionContext);
            var queryString = new NameValueCollection();
            string query = actionContext.Request.RequestUri.Query;
            if (!string.IsNullOrEmpty(query)) {
                query = query.Substring(1);
                string[] parts = query.Split('&');
                foreach (var item in parts) {
                    string[] pair = item.Split('=');
                    if (pair.Length == 2) {
                        queryString.Add(pair[0], pair[1]);
                    }
                }
            }
            long timestamp = 0;
            string t = queryString["timestamp"];
            if (!string.IsNullOrEmpty(t)) {
                long.TryParse(t, out timestamp);
            }
            ClientSignData clientSign = new ClientSignData(queryString["loginName"], queryString["sign"], timestamp);
            ISignableData input = null;
            var actionParameters = actionContext.ActionDescriptor.GetParameters();
            if (actionParameters.Count == 1 && typeof(ISignableData).IsAssignableFrom(actionParameters[0].ParameterType)) {
                input = (ISignableData)actionContext.ActionArguments.First().Value;
            }
            Type returnType = actionContext.ActionDescriptor.ReturnType;
            var tMethodInfo = _methodInfo.MakeGenericMethod(returnType);
            string message = null;
            var parameters = new object[] { clientSign, input, null, null };
            bool isValid = (bool)tMethodInfo.Invoke(null, parameters);
            ResponseBase response = (ResponseBase)parameters[2];// 因为调用的是带out参数的方法
            UserData user = (UserData)parameters[3];// 因为调用的是带out参数的方法
            if (isValid) {
                isValid = OnAuthorization(user, out message);
            }
            if (!isValid) {
                if (response != null && !string.IsNullOrEmpty(message)) {
                    response.Description = message;
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(VirtualRoot.JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
                };
            }
            else {
                actionContext.ControllerContext.RouteData.Values["_user"] = user;
            }
        }

        private static bool IsValidUser<TResponse>(ClientSignData query, ISignableData data, out TResponse response, out UserData user) where TResponse : ResponseBase, new() {
            user = null;
            if (!WebApiRoot.UserSet.IsReadied) {
                string message = "服务器用户集启动中，请稍后";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            if (!Timestamp.IsInTime(query.Timestamp)) {
                response = ResponseBase.Expired<TResponse>();
                return false;
            }
            if (!string.IsNullOrEmpty(query.LoginName)) {
                user = WebApiRoot.UserSet.GetUser(query.UserId);
            }
            if (user == null) {
                string message = "用户不存在";
                response = ResponseBase.NotExist<TResponse>(message);
                return false;
            }
            string mySign = RpcUser.CalcSign(user.LoginName, user.Password, query.Timestamp, data);
            if (query.Sign != mySign) {
                string message = "签名错误：1. 可能因为登录名或密码错误；2. 可能因为软件版本过期需要升级软件，请将软件升级到最新版本再试。";
                response = ResponseBase.Forbidden<TResponse>(message);
                return false;
            }
            response = null;
            return true;
        }
    }
}
