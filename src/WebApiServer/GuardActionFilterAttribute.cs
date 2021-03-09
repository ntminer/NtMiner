using System;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NTMiner {
    /// <summary>
    /// 门卫
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GuardActionFilterAttribute : ActionFilterAttribute {
        public GuardActionFilterAttribute() { }

        public override void OnActionExecuting(HttpActionContext actionContext) {
            base.OnActionExecuting(actionContext);
            string ip = actionContext.Request.GetRemoteIp();
            if (IPAddress.TryParse(ip, out IPAddress remoteIp)) {
                VirtualRoot.RaiseEvent(new WebApiRequestEvent(remoteIp));
            }
            string controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = $"{controllerName}.{actionContext.ActionDescriptor.ActionName}";
            AppRoot.Action(actionName);
        }
    }
}
