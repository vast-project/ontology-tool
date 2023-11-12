using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAST.Ontology.Database.Models
{
    public class Context
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Item> SecondaryItems { get; set; }
        public List<Item> PrimaryItems { get; set; }
        public List<ItemLink> PrimaryItemLinks { get; set; }
    }
}
