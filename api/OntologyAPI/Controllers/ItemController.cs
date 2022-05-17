using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OntologyAPI.Controllers
{
    public class StatDataTile
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string[] data { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private VastOntologyContext _ontologyContext = null;
        public ItemController(VastOntologyContext ontologyContext)
        {
            _ontologyContext = ontologyContext;
        }

        // GET: api/<ItemController>
        [HttpGet("")]
        public IEnumerable<object> Get(ItemType type = ItemType.Keyword, string? search = null, int page = 1, int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            
            {
                var items = _ontologyContext.Items.Where(i => !i.IsDeleted);
                if (type != ItemType.Unknown)
                {
                    items = items.Where(i => i.ItemType == type);
                }
                if (!string.IsNullOrWhiteSpace(search))
                {
                    string normalizedSearch = search.ToLower().Trim();
                    items = items.Where(i => i.Name.ToLower().Contains(normalizedSearch));
                }
                var results = items.OrderBy(i => i.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(i => new
                {
                    i.Name,
                    i.Value,
                    i.Description,
                    i.ItemType,
                    i.Id,
                    i.IsImported,
                    i.IsInSchema
                }).ToList();

                return results;
            }
        }

        [HttpGet("stats")]
        public object GetStats()
        {
            
            {
                int annotations = _ontologyContext.Annotations.Count(i => !i.IsDeleted);
                int docs = _ontologyContext.Documents.Count();
                int cols = _ontologyContext.Collections.Count();
                int keywords = _ontologyContext.Items.Count(i => !i.IsDeleted && i.ItemType == ItemType.Keyword);
                int concepts = _ontologyContext.Items.Count(i => !i.IsDeleted && i.ItemType == ItemType.Concept);

                return new StatDataTile[]
                {
                    new StatDataTile {title=annotations.ToString(), subtitle = "annotations", data=new string[] { $"{docs} documents", $"{cols} collections" }},
                    new StatDataTile {title=keywords.ToString(), subtitle = "keywords", data=new string[0] },
                    new StatDataTile {title=concepts.ToString(), subtitle = "concepts", data=new string[0]},
                };
            }
        }

        [HttpGet("recent")]
        public IEnumerable<object> GetRecent()
        {
            
            {
                var annotations = _ontologyContext.Annotations.Where(i => !i.IsDeleted).OrderByDescending(i => i.Created).Take(3)
                    .Select(i => new
                    {
                        description = i.Description,
                        collection = i.Document.Collection.Name,
                        document = i.Document.Name,
                    });

                return annotations.ToList();
            }
        }

    }
}
