using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class ItemLink
    {
        public int Id { get; set; }
        public Item Source { get; set; }
        public Item Target { get; set; }
        public RelationshipType RelationshipType { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
