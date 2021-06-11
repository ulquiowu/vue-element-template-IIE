using iie_erp_assist_server.Authorities;
using iie_erp_assist_server.DataTransferObjects;
using iie_erp_assist_server.Models;
using iie_erp_assist_server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Controllers
{
    /// <summary>
    /// 授权模块
    /// </summary>
    public class LoginController : BaseController
    {
        /// <summary>
        /// 获取授权 token 字符串
        /// </summary>
        /// <param name="usercode">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="plant">厂区编码 XUS 或 IIE</param>
        /// <returns></returns>
        [HttpGet("{usercode}/{password}/{plant}")]
        public async Task<object> Get(string usercode, string password, string plant)
        {
            Login login = new();
            login.UserCode = usercode;
            login.Password = password;
            // 验证用户名和密码
            string sql = "select t.code, t.name from YunChiERPWQ.Security.UsersMaster t where t.code = @code and t.password = @password ";
            if (plant == "XUS")
            {
                sql = "select t.code, t.name from YunChiERP.Security.UsersMaster t where t.code = @code and t.password = @password ";
            }
            SqlParameter[] ps = new SqlParameter[]
            {
                new SqlParameter("@code", SqlDbType.VarChar),
                new SqlParameter("@password", SqlDbType.VarChar),
            };
            ps[0].Value = usercode;
            ps[1].Value = password;

            try
            {
                using AssistContext DB = new();
                DataTable dt = DBContextEx.SqlQuery(DB.Database, CommandType.Text, sql, ps);
                // 用户名或者密码错误 直接返回
                if (dt.Rows.Count <= 0)
                {
                    var response = FormatApiResult<Login>.result(login, new Meta
                    {
                        Msg = "用户名或密码错误",
                        Status = Services.StatusCode.NOT_FOUND
                    });
                    return await FinalApiResult<Login>.Get(response);
                }
                // 验证成功，生成 token 信息并保存到内存中
                // 取第一行数据
                DataRow dr = dt.Rows[0];
                TokenDTO token = new();
                token.usercode = dr["code"].ToString();
                token.username = dr["name"].ToString();
                TokenDTO finalToken = AutToken.IssueJWT(token, new TimeSpan(0, 0, 30, 0), new TimeSpan(1, 0, 0, 0)); // 生成Token并存储到内存,TimeSpan(day,hour,minute,second)
                // 验证token是否已正确存入内存
                var tokenMemory = AutMemoryCache.Get(finalToken.token);
                if (tokenMemory == null)
                {
                    var response = FormatApiResult<Login>.result(login, new Meta
                    {
                        Msg = "密钥生成失败,请再次尝试!",
                        Status = Services.StatusCode.INTERNAL_SERVER_ERROR
                    });
                    return await FinalApiResult<Login>.Get(response);
                }
                token.token = finalToken.token;
                // Token 生成成功
                var re = FormatApiResult<TokenDTO>.result(token, new Meta
                {
                    Msg = "登录成功!",
                    Status = Services.StatusCode.OK
                });
                return await FinalApiResult<TokenDTO>.Get(re);
            }
            catch (Exception ex)
            {
                var re = FormatApiResult<Login>.result(login, new Meta
                {
                    Msg = "服务器内部错误!" + ex.Message.ToString(),
                    Status = Services.StatusCode.INTERNAL_SERVER_ERROR
                });

                return await FinalApiResult<Login>.Get(re);
            }
        }
    }
}
