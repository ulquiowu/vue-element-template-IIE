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
    /// 申请单模块
    /// </summary>
    [Authorize(Policy = "Client")]
    public class PreApplyController : BaseController
    {
        /// <summary>
        /// 申请单明细分页查询
        /// </summary>
        /// <param name="plant">必填项:厂区编码 XUS 或 IIE</param>
        /// <param name="pagenum">必填项:当前页码</param>
        /// <param name="pagesize">必填项:每页显示数</param>
        /// <param name="begindate">必填项:开始时间</param>
        /// <param name="enddate">必填项:结束时间</param>
        /// <param name="projectcode">项目号</param>
        /// <param name="prodcode">物料编码</param>
        /// <returns></returns>
        [HttpGet("{plant}/{pagenum}/{pagesize}/{begindate}/{enddate}")]
        public async Task<object> Get(string plant, int pagenum, int pagesize, DateTime begindate, DateTime enddate, string projectcode, string prodcode)
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
                string sql = @"SELECT A2.BillNum , A2.Item , A2.ProdCode , A2.ProdName , 
                            A2.AllowQty , A2.FinQty ,A1.ProjectCode
                            FROM YunChiERPWQ.Stock.PreApplyMaster A1
                            LEFT JOIN YunChiERPWQ.Stock.PreApplyDetail A2 ON A2.BillNum = A1.BillNum
                            WHERE A1.BillState = 6 AND ISNULL(A2.CancelQty, 0) = 0 AND(ISNULL(A2.AllowQty, 0) - ISNULL(A2.FinQty, 0)) > 0
                            AND CONVERT(DATETIME, A1.BillDate, 120) BETWEEN CONVERT(DATETIME, @BeginDate, 120) AND CONVERT(DATETIME, @EndDate, 120)
                            AND ISNULL(A1.ProjectCode, '') LIKE '%' + @ProjectCode + '%'
                            AND A2.ProdCode LIKE '%' + @ProdCode + '%' ORDER BY A1.BuildDate";

                if (plant == "XUS")
                {
                    sql = @"SELECT A2.BillNum , A2.Item , A2.ProdCode , A2.ProdName , 
                            A2.AllowQty , A2.FinQty ,A1.ProjectCode
                            FROM YunChiERP.Stock.PreApplyMaster A1
                            LEFT JOIN YunChiERP.Stock.PreApplyDetail A2 ON A2.BillNum = A1.BillNum
                            WHERE A1.BillState = 6 AND ISNULL(A2.CancelQty, 0) = 0 AND(ISNULL(A2.AllowQty, 0) - ISNULL(A2.FinQty, 0)) > 0
                            AND CONVERT(DATETIME, A1.BillDate, 120) BETWEEN CONVERT(DATETIME, @BeginDate, 120) AND CONVERT(DATETIME, @EndDate, 120)
                            AND ISNULL(A1.ProjectCode, '') LIKE '%' + @ProjectCode + '%'
                            AND A2.ProdCode LIKE '%' + @ProdCode + '%' ORDER BY A1.BuildDate";
                }
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@BeginDate", begindate),
                    new SqlParameter("@EndDate", enddate),
                    new SqlParameter("@ProjectCode", projectcode ?? ""),
                    new SqlParameter("@ProdCode", prodcode ?? ""),
                };

                using AssistContext DB = new();
                //所有数据
                var totalList = DBContextEx.SqlQuery<PreApply>(DB.Database, CommandType.Text, sql, ps).ToList();
                //分页数据
                List<PreApply> pageApplies = totalList.Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
                ApiPageResult pageResult = new();
                if (pageApplies.Count == 0)
                {
                    var response = FormatApiResult<object>.result(null, new Meta
                    {
                        Msg = "没有找到相应的数据",
                        Status = Services.StatusCode.NOT_FOUND
                    });
                    return await FinalApiResult<object>.Get(response);
                }
                else
                {
                    pageResult.PageNum = pagenum;
                    pageResult.Total = totalList.Count;
                    pageResult.Details.AddRange(pageApplies);
                    pageResult.meta = new()
                    {
                        Msg = "获取成功",
                        Status = Services.StatusCode.OK
                    };
                    return pageResult;

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
