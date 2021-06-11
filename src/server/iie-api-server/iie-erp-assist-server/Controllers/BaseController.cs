using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Controllers
{
    /// <summary>
    /// 自定义路由模板
    /// 解决swagger文档 No operations defined in spec!问题
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}
