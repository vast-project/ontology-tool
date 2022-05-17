using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType ItemType { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsImported { get; set; }
        public bool IsInSchema { get; set; }
        public DateTime LastSyncTime { get; set; }
        public List<ItemLink> TargetLinks { get; set; }
        public List<ItemLink> SourceLinks { get; set; }
        public List<Annotation> Annotations { get; set; }
    }
}
