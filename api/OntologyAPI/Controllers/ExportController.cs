using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

namespace OntologyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private VastOntologyContext _ontologyContext = null;
        public ExportController(VastOntologyContext ontologyContext)
        {
            _ontologyContext = ontologyContext;
        }

        [HttpGet("get-statement-csv")]
        public FileResult GetStatementCsv(string? search = null, int? sourceId = null, int? targetId = null,
            int context = 0)
        {
            StringBuilder sb = new StringBuilder();
            IQueryable<ItemLink> items = _ontologyContext.ItemLinks;

            //If a context is provided, filter by the context
            if (context > 0)
            {
                items = items.Where(i => i.PrimaryContext.Id == context);
            }

            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(i =>
                    (i.Source.Name.ToLower().Contains(search)) || i.Target.Name.ToLower().Contains(search));
            }

            if (sourceId != null)
            {
                items = items.Where(i => i.Source.Id == sourceId);
            }

            if (targetId != null)
            {
                items = items.Where(i => i.Target.Id == targetId);
            }

            var results = items.Select(i => new
            {
                i.Id,
                i.AuthorId,
                i.AuthorName,
                Source = new { i.Source.Id, i.Source.ItemType, i.Source.Name },
                Target = new { i.Target.Id, i.Target.ItemType, i.Target.Name },
                RelationshipType = new { i.RelationshipType.Name, i.RelationshipType.OntologyId, i.RelationshipType.Id },
                Context = new { Id = i.PrimaryContext.Id, Name = i.PrimaryContext.Title }
            }).ToList();

            sb.AppendLine(
                $"StatementId;AuthorId;AuthorName;SourceId;SourceType;SourceName;RelationshipId;RelationshipOntId;RelationshipName;TargetId;TargetType;TargetName;ContextId;ContextName");
            foreach (var result in results)
            {
                sb.AppendLine(
                    $"{result.Id};---;---;{result.Source.Id};{result.Source.ItemType};{result.Source.Name};{result.RelationshipType.Id};{result.RelationshipType.OntologyId};{result.RelationshipType.Name};{result.Target.Id};{result.Target.ItemType};{result.Target.Name};{result.Context.Id};{result.Context.Name}");
            }

            return new FileContentResult(Encoding.UTF8.GetBytes(sb.ToString()), "text/plain; charset=UTF-8");
        }
    }
}
