using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Annotation.ProxyTest
{
    public class CsvRow
    {
        public int TypeId { get; set; }
        public string AuthorId { get; set; }
        public DateTime Inserted { get; set; }
        public int PrimaryContextId { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string ContextName { get; set; }
    }

}
