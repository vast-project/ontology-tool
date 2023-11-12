using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VAST.Ontology.Database
{
    public class OntologyContextFactory : IDesignTimeDbContextFactory<VastOntologyContext>
    {
        public VastOntologyContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VastOntologyContext>();
            optionsBuilder.UseNpgsql("Data Source=");

            return new VastOntologyContext(optionsBuilder.Options);
        }
    }
}
