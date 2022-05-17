using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

namespace OntologyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnotationController : ControllerBase
    {
        [HttpGet("item")]
        public IEnumerable<object> Get(string? search = null, int? document = null, int? keywordConcept = null, int page = 1, int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            using (VastOntologyContext context = new VastOntologyContext())
            {
                var items = context.Annotations.Where(i => !i.IsDeleted);
                if (document != null)
                {
                    items = items.Where(i => i.Document.Id == document);
                }
                if (keywordConcept != null)
                {
                    items = items.Where(i => i.AnnotationItem.Any(ai => ai.Id == keywordConcept));
                }
                if (!string.IsNullOrWhiteSpace(search))
                {
                    string normalizedSearch = search.ToLower().Trim();
                    items = items.Where(i =>
                        i.Description.ToLower().Contains(normalizedSearch) || i.AnnotationItem.Any(ai =>
                            !ai.IsDeleted && ai.Name.ToLower().Contains(normalizedSearch)));
                }

                var results = items.OrderBy(i => i.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(i => new
                {
                    DocumentName = i.Document.Name,
                    CollectionName = i.Document.Collection.Name,
                    i.Description,
                    i.IsImported,
                    i.OriginalId,
                    Keywords = i.AnnotationItem.Where(ai => !ai.IsDeleted && ai.ItemType == ItemType.Keyword)
                        .Select(ai => new { ai.Id, ai.Name, ai.Value, ai.IsImported, ai.IsInSchema }),
                    Concepts = i.AnnotationItem.Where(ai => !ai.IsDeleted && ai.ItemType == ItemType.Concept)
                        .Select(ai => new { ai.Id, ai.Name, ai.Value, ai.IsImported, ai.IsInSchema })
                }).ToList();

                return results;
            }
        }

        [HttpGet("collection")]
        public IEnumerable<object> GetCollections(string? search = null, int page = 1, int pageSize = 100)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            using (VastOntologyContext context = new VastOntologyContext())
            {
                IQueryable<Collection> items = context.Collections;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    string normalizedSearch = search.ToLower().Trim();
                    items = items.Where(i =>
                        i.Name.ToLower().Contains(normalizedSearch));
                }

                var results = items.OrderBy(i => i.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(i => new
                {
                    i.Id,
                    i.Name,
                }).ToList();

                return results;
            }
        }

        [HttpGet("document")]
        public IEnumerable<object> GetDocuments(string? search = null, int? collection = null, int page = 1, int pageSize = 100)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            using (VastOntologyContext context = new VastOntologyContext())
            {
                IQueryable<Document> items = context.Documents;
                if (collection != null)
                {
                    items = items.Where(i => i.Collection.Id == collection);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string normalizedSearch = search.ToLower().Trim();
                    items = items.Where(i =>
                        i.Name.ToLower().Contains(normalizedSearch));
                }

                var results = items.OrderBy(i => i.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(i => new
                {
                    i.Id,
                    i.Name,
                }).ToList();

                return results;
            }
        }
    }

}
