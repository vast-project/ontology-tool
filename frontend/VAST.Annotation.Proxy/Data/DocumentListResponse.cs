using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.Proxy.Data
{

    public class DocumentListResponse
    {
        public bool success { get; set; }
        public DocumentItem[] data { get; set; }
    }

    public class DocumentItem
    {
        public int id { get; set; }
        public int collection_id { get; set; }
        public string name { get; set; }
        public string external_name { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public string data_text { get; set; }
        public object data_binary { get; set; }
        public string encoding { get; set; }
        public string handler { get; set; }
        public string visualisation_options { get; set; }
        public int owner_id { get; set; }
        public string owner_email { get; set; }
        public object metadata { get; set; }
        public int version { get; set; }
        public string updated_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}
