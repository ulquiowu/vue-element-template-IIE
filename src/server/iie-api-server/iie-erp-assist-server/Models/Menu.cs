using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Models
{
    public class Menu
    {
        public string id { get; set; }
        public string authName { get; set; }
        public string path { get; set; }
        public List<Children> children { get; set; }
 
    }

    public class Children
    {
        public string id { get; set; }
        public string authName { get; set; }
        public string path { get; set; }
        public List<string> children { get; set; }
    }
}
