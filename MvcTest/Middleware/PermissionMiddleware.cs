using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcTest.Middleware
{
    /// <summary>
    /// 权限中间件
    /// </summary>
    public class PermissionMiddleware
    {
        /// <summary>
        /// 管道代理对象
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 权限中间件的配置选项
        /// </summary>
        private readonly PermissionMiddlewareOption _option;

        /// <summary>
        /// 用户权限集合
        /// </summary>
        internal static List<UserPermission> _userPermissions;

        public PermissionMiddleware(RequestDelegate next, PermissionMiddlewareOption option)
        {
            _option = option;
            _next = next;
            _userPermissions = option.UserPermissions;
        }

        /// <summary>
        /// 调用管道
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            //请求Url

            var requestUrl = context.Request.Path.Value.ToLower();

            var isAuthenticated = context.User.Identity.IsAuthenticated;
            //是否经过验证
            if (isAuthenticated)
            {
                if (_userPermissions.GroupBy(g => g.Url).Where(w => w.Key.ToLower().Equals(requestUrl)).Count() > 0)
                {
                    var userName = context.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Sid).Value;
                    if (_userPermissions.Where(w => w.UserName.Equals(userName) && w.Url.ToLower().Equals(requestUrl)).Count() > 0)
                    {
                        return this._next(context);
                    }
                    else
                    {
                        //跳转到拒绝页面
                        context.Response.Redirect(_option.NoPemission);
                    }
                }
            }
            return this._next(context);
        }
    }

    /// <summary>
    /// 权限中间件选项
    /// </summary>
    public class PermissionMiddlewareOption
    {
        /// <summary>
        /// 登录Action
        /// </summary>
        public string LoginAction { get; set; }
        /// <summary>
        /// 无权限导航Action
        /// </summary>
        public string NoPemission { get; set; }
        /// <summary>
        /// 用户权限集合
        /// </summary>
        public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    }

    /// <summary>
    /// 用户权限
    /// </summary>
    public class UserPermission
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 请求Url
        /// </summary>
        public string Url { get; set; }
    }


    /// <summary>
    /// 中间件扩展
    /// </summary>
    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermission(this IApplicationBuilder builder, PermissionMiddlewareOption option)
        {
            return builder.UseMiddleware<PermissionMiddleware>(option);
        }
    }
}
