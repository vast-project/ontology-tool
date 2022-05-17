using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.Proxy.Data
{

    public class CollectionListResponse
    {
        public bool success { get; set; }
        public CollectionItem[] data { get; set; }
    }

    public class CollectionItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string handler { get; set; }
        public string encoding { get; set; }
        public int owner_id { get; set; }
        public int confirmed { get; set; }
        public int is_owner { get; set; }
        public int document_count { get; set; }
    }

}
