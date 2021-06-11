using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Services
{
    public class ApiPageResult
    {
        public int Total { get; set; }
        public int PageNum { get; set; }
        public List<object> Details { get; set; } = new List<object>();
        public Meta meta { get; set; } = new Meta();
    }
}
