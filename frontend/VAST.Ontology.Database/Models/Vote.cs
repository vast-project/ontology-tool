using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public ItemLink ItemLink { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool DuplicateLink { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Negative { get; set; }
    }
}
