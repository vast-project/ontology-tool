using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.Proxy.Data
{

    public class AnnotationListResponse
    {
        public bool success { get; set; }
        public AnnotationItem[] data { get; set; }
    }

    public class AnnotationItem
    {
        public string _id { get; set; }
        public int collection_id { get; set; }
        public int document_id { get; set; }
        public int owner_id { get; set; }
        public string type { get; set; }
        public Span[] spans { get; set; }
        public Attribute[] attributes { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string created_at { get; set; }
        public string annotator_id { get; set; }
        public string updated_at { get; set; }
        public string document_attribute { get; set; }
        public string collection_name { get; set; }
        public string document_name { get; set; }
    }

    public class Span
    {
        public string segment { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }

    public class Attribute
    {
        public string name { get; set; }
        public object value { get; set; }
    }

}
