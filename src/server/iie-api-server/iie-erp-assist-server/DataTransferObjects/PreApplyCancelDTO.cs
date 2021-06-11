using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.DataTransferObjects
{
    public class PreApplyCancelDTO
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string ReturnInfo { get; set; }
    }
}
