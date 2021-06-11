using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Services
{
    /// <summary>
    /// 响应结果对象
    /// </summary>
    public class ApiResult
    {
        public List<object> data { get; set; } = new List<object>();
        public Meta meta { get; set; } = new Meta();
    }
}
