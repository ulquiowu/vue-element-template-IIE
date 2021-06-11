using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Services
{
    /// <summary>
    /// 响应元素
    /// </summary>
    public class Meta
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public StatusCode Status { get; set; }
    }
}
