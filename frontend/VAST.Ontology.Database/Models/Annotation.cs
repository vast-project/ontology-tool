using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class Annotation
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int DocumentId { get; set; }
        public string OriginalId { get; set; }
        public List<Item> AnnotationItem { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsImported { get; set; }
        public Document Document { get; set; }
    }
}
