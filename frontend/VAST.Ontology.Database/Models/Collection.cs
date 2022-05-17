using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OriginalId { get; set; }

        public List<Document> Documents { get; set; }
    }
}
