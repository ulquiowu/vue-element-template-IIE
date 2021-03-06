using iie_erp_assist_server.DataTransferObjects;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Authorities
{
    /// <summary>
    /// 令牌类
    /// </summary>
    public class AutToken
    {
        /// <summary>
        /// 获取JWT字符串并存入缓存
        /// </summary>
        /// <param name="tokenModel">Token对象</param>
        /// <param name="expiresSliding">时间戳</param>
        /// <param name="expiresAbsoulte">绝对过期时间</param>
        /// <returns></returns>
        public static TokenDTO IssueJWT(TokenDTO tokenModel, TimeSpan expiresSliding, TimeSpan expiresAbsoulte)
        {
            DateTime UTC = DateTime.UtcNow;
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//JWT ID,JWT的唯一标识
                new Claim(JwtRegisteredClaimNames.Iat, UTC.ToString(), ClaimValueTypes.Integer64),//Issued At，JWT颁发的时间，采用标准unix时间，用于验证过期
            };

            JwtSecurityToken jwt = new(
            issuer: "WXIIE",//jwt签发者,非必须
            audience: tokenModel.usercode,//jwt的接收该方，非必须
            claims: claims,//声明集合
            expires: UTC.AddHours(12),//指定token的生命周期，unix时间戳格式,非必须
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CIM_Secret_Key11111111111111111")), SecurityAlgorithms.HmacSha256));//使用私钥进行签名加密
            var encodedJwt = "Bearer " + new JwtSecurityTokenHandler().WriteToken(jwt);//生成最后的JWT字符串
            tokenModel.token = encodedJwt; // 把JWT字符串赋值给tokenModel的Token字体
            AutMemoryCache.AddMemoryCache(encodedJwt, tokenModel, expiresSliding, expiresAbsoulte);//将JWT字符串和tokenModel作为key和value存入缓存
            return tokenModel;
        }
    }
}
