using iie_erp_assist_server.Authorities;
using iie_erp_assist_server.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Services
{
    /// <summary>
    /// Token验证授权中间件
    /// </summary>
    public class TokenAuthHelper
    {
        /// <summary>
        /// http委托
        /// </summary>
        private readonly RequestDelegate _next;

        //public object AutMemoryCache { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public TokenAuthHelper(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// 验证授权
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;
            var path = httpContext.Request.Path;

            //检测是否包含'Authorization'请求头，如果不包含返回context进行下一个中间件，用于访问不需要认证的API
            if (!headers.ContainsKey("Authorization") || path.Value.ToString().Contains("Login"))
            {
                return _next(httpContext);
            }
            var tokenStr = headers["Authorization"];
            try
            {
                //string jwtStr = tokenStr.ToString().Substring("Bearer ".Length).Trim();
                string jwtStr = tokenStr.ToString().Trim();
                //验证缓存中是否存在该jwt字符串
                if (!AutMemoryCache.Exists(jwtStr))
                {
                    return httpContext.Response.WriteAsync("您尚未登录,请登录后重新尝试");
                }

                TokenDTO tm = ((TokenDTO)AutMemoryCache.Get(jwtStr));
                //提取tokenModel中的Email属性进行authorize认证
                List<Claim> lc = new List<Claim>();
                Claim c = new Claim("Client" + "Type", "Client"); // 后续需要传入实际用户权限
                lc.Add(c);
                ClaimsIdentity identity = new ClaimsIdentity(lc);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;

                return _next(httpContext);
            }
            catch (Exception ex)
            {
                return httpContext.Response.WriteAsync("token验证异常" + ex.ToString());
            }
        }

        /// <summary>
        /// 删除对应jwtToken的内存Token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static Task DelToken(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;
            if (headers.ContainsKey("Authorization"))
            {
                try
                {
                    var tokenStr = headers["Authorization"];
                    string jwtStr = tokenStr.ToString().Substring("Bearer ".Length).Trim();
                    bool isDel = AutMemoryCache.DelMemoryCache(jwtStr);
                    if (isDel)
                    {
                        return httpContext.Response.WriteAsync("您已成功注销");
                    }
                    else
                    {
                        return httpContext.Response.WriteAsync("注销失败,请再次尝试");
                    }
                }
                catch (Exception ex)
                {
                    return httpContext.Response.WriteAsync("注销失败,请检查!" + ex.ToString());
                }
            }
            else
            {
                return httpContext.Response.WriteAsync("您尚未登录!");
            }
        }
    }
}
