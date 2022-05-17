using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OriginalId { get; set; }
        
        public List<Annotation> Annotations { get; set; }
        public Collection Collection { get; set; }
    }
}
