using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.Proxy.Data
{

    public class SchemaResponse
    {
        public Group[] groups { get; set; }
    }

    public class Group
    {
        public string group { get; set; }
        public string[] values { get; set; }
        public string[] labels { get; set; }
        public string[] descriptions { get; set; }
        public Value_Options[][] value_options { get; set; }
    }

    public class Value_Options
    {
        public string name { get; set; }
        public string label { get; set; }
        public string rowspan { get; set; }
        public string columnspan { get; set; }
        public string type { get; set; }
        public string group { get; set; }
    }

}
