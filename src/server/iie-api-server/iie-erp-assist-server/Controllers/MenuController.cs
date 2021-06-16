using iie_erp_assist_server.DataTransferObjects;
using iie_erp_assist_server.Models;
using iie_erp_assist_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Controllers
{
    /// <summary>
    /// 菜单模块
    /// </summary>
    [Authorize(Policy = "Client")]
    public class MenuController : BaseController
    {

        [HttpGet("{username}")]
        public async Task<object> Get(string username)
        {

            try
            {
                List<Menu> menus = new();
                for (int i = 0; i < 3; i++)
                {
                    Menu menu = new();
                    menu.id = i.ToString();
                    menu.authName = "菜单" + i.ToString();
                    menu.children = new();
                    for (int j = 0; j < 3; j++)
                    {
                        Children children = new();
                        children.id = i.ToString()+"-"+j.ToString();
                        children.authName = "菜单" + i.ToString() + "-" + j.ToString();
                        children.path = i.ToString() + "-" + j.ToString();
                        menu.children.Add(children);
                    }              
                    menus.Add(menu);
                }
                ApiPageResult pageResult = new();
                pageResult.Details.AddRange(menus);
                pageResult.meta = new()
                {
                    Msg = "获取成功",
                    Status = Services.StatusCode.OK
                };
                return pageResult;
            }
            catch (Exception ex)
            {
                var res = FormatApiResult<object>.result(null, new Meta
                {
                    Msg = "服务器内部错误:" + ex.Message.ToString(),
                    Status = Services.StatusCode.INTERNAL_SERVER_ERROR
                }); ;

                return await FinalApiResult<object>.Get(res);
            }
        }

        /// <summary>
        /// 取消请购信息
        /// </summary>
        /// <param name="plant">必填项:厂区编码 XUS 或 IIE</param>
        /// <param name="billnum">必填项:请购单号</param>
        /// <param name="item">必填项:请购单项</param>
        /// <returns></returns>
        [HttpPut("{plant}/{billnum}/{item}")]
        public async Task<object> Put(string plant, string billnum, int item)
        {
            if (plant != "XUS" && plant != "IIE")
            {
                var res = FormatApiResult<object>.result(null, new Meta
                {
                    Msg = "无效的厂区编码(厂区编码 XUS 或 IIE)",
                    Status = Services.StatusCode.FORBIDDEN
                }); ;
                return await FinalApiResult<object>.Get(res);
            }
            try
            {
                if (string.IsNullOrEmpty(billnum) || string.IsNullOrEmpty(item.ToString()))
                {
                    var res = FormatApiResult<object>.result(null, new Meta
                    {
                        Msg = "操作失败:缺少必要的参数",
                        Status = Services.StatusCode.FORBIDDEN
                    }); ;
                    return await FinalApiResult<object>.Get(res);
                }

                string sql = "EXEC YunChiERPWQ.Stock.spPreApplyCancelByItem1 @SystemName, @BillNum, @Item , @UserCode";
                if (plant == "XUS")
                {
                    sql = "EXEC YunChiERP.Stock.spPreApplyCancelByItem1 @SystemName, @BillNum, @Item , @UserCode";
                }
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@SystemName", ""),
                    new SqlParameter("@BillNum", billnum),
                    new SqlParameter("@Item", item),
                    new SqlParameter("@UserCode", "admin")
                };

                PreApplyDTO preApplyDTO = new();
                preApplyDTO.BillNum = billnum;
                preApplyDTO.Item = item;
                preApplyDTO.UserCode = "admin";

                using AssistContext DB = new();
                PreApplyCancelDTO preApplyCancelDTO = DBContextEx.SqlQuery<PreApplyCancelDTO>(DB.Database, CommandType.Text, sql, ps).FirstOrDefault();
                if (preApplyCancelDTO.ErrCode == 0)
                {
                    var res = FormatApiResult<PreApplyDTO>.result(preApplyDTO, new Meta
                    {
                        Msg = "取消成功",
                        Status = Services.StatusCode.OK
                    }); ;
                    return await FinalApiResult<PreApplyDTO>.Get(res);
                }
                else
                {
                    var res = FormatApiResult<PreApplyDTO>.result(preApplyDTO, new Meta
                    {
                        Msg = "取消失败:" + preApplyCancelDTO.ErrMsg,
                        Status = Services.StatusCode.FORBIDDEN
                    }); ;
                    return await FinalApiResult<PreApplyDTO>.Get(res);
                }
            }
            catch (Exception ex)
            {
                var res = FormatApiResult<object>.result(null, new Meta
                {
                    Msg = "服务器内部错误:" + ex.Message.ToString(),
                    Status = Services.StatusCode.INTERNAL_SERVER_ERROR
                }); ;
                return await FinalApiResult<object>.Get(res);
            }
        }
    }
}
