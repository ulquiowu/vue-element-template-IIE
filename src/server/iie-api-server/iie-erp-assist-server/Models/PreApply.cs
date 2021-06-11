using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Models
{
    public class PreApply
    {
        public string BillNum { get; set; }
        public int Item { get; set; }
        public string ProdCode { get; set; }
        public string ProdName { get; set; }
        public double AllowQty { get; set; }
        public double FinQty { get; set; }
        public string ProjectCode { get; set; }
    }
}
