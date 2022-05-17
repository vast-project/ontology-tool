using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OntologyAPI.Controllers
{
    public class StatementData
    {
        public int SourceId { get; set; }
        public int? TargetId { get; set; }
        public int RelationshipId { get; set; }
        public string? TargetName { get; set; }
        public string AuthorId { get; set; }
    }
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ontology")]
    [ApiController]
    public class StatementController : ControllerBase
    {
        private string UserName
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    throw new InvalidOperationException("User needs to be authenticated in order to get the identity");

                var claimsIdentity = User.Identity as ClaimsIdentity;

                if (claimsIdentity == null)
                    throw new InvalidOperationException("User needs to be authenticated using claims");

                var username = claimsIdentity.Claims
                    .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    .FirstOrDefault();

                if (username == null)
                    throw new InvalidOperationException("User claim does not contain the username");

                return username.Value;
            }
        }

        // GET: api/<StatementController>
        [HttpGet("")]
        public IEnumerable<object> GetAll(string? search = null, int? sourceId = null, int? targetId = null)
        {
            using (VastOntologyContext context = new VastOntologyContext())
            {
                var items = context.ItemLinks;

                return items.Select(i => new
                {
                    i.Id,
                    i.AuthorId,
                    i.AuthorName,
                    Source = new { i.Source.Id, i.Source.ItemType, i.Source.Name },
                    Target = new { i.Target.Id, i.Target.ItemType, i.Target.Name },
                    RelationshipType = new { i.RelationshipType.Name, i.RelationshipType.OntologyId, i.RelationshipType.Id }
                }).ToList();
            }
        }

        [HttpGet("me")]
        public IEnumerable<object> GetMine(string? search = null, int? sourceId = null, int? targetId = null)
        {
            using (VastOntologyContext context = new VastOntologyContext())
            {
                var items = context.ItemLinks.Where(i => i.AuthorId == UserName || i.Votes.Any(v => v.AuthorId == UserName && v.DuplicateLink == true));

                return items.Select(i => new
                {
                    i.Id,
                    i.AuthorId,
                    i.AuthorName,
                    Source = new { i.Source.Id, i.Source.ItemType, i.Source.Name },
                    Target = new { i.Target.Id, i.Target.ItemType, i.Target.Name },
                    RelationshipType = new { i.RelationshipType.Name, i.RelationshipType.OntologyId, i.RelationshipType.Id }
                }).ToList();
            }
        }

        [HttpGet("other")]
        public IEnumerable<object> GetOthers(string? search = null, int? sourceId = null, int? targetId = null)
        {
            using (VastOntologyContext context = new VastOntologyContext())
            {
                var items = context.ItemLinks.Where(i => i.AuthorId != UserName);

                return items.Select(i => new
                {
                    i.Id,
                    i.AuthorId,
                    i.AuthorName,
                    Source = new { i.Source.Id, i.Source.ItemType, i.Source.Name },
                    Target = new { i.Target.Id, i.Target.ItemType, i.Target.Name },
                    RelationshipType = new { i.RelationshipType.Name, i.RelationshipType.OntologyId, i.RelationshipType.Id }
                }).ToList();
            }
        }

        [HttpGet("rel-types")]
        public IEnumerable<object> GetRelType(string? search = null)
        {
            using (VastOntologyContext context = new VastOntologyContext())
            {
                IQueryable<RelationshipType> items = context.RelationshipTypes;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    items = items.Where(x => x.Name.ToLower().Contains(search.ToLower()));
                }

                var results = items.Select(i => new { i.Id, i.Name, i.OntologyId });
                return results.ToList();
            }
        }

        // PUT api/<StatementController>/5
        [HttpPost()]
        public void Post([FromBody] StatementData value)
        {
            if (value == null || (value.TargetId == null && value.TargetName == null))
                throw new ArgumentNullException(nameof(value));

            using (VastOntologyContext context = new VastOntologyContext())
            {
                ItemLink addItem;
                if (value.TargetId != null)
                {
                    var existingItem = context.ItemLinks.FirstOrDefault(il => il.Source.Id == value.SourceId && il.Target.Id == value.TargetId && il.RelationshipType.Id == value.RelationshipId);
                    if (existingItem != null)
                    {
                        if (existingItem.AuthorId != UserName)
                        {
                            var existingVote = context.Votes.FirstOrDefault(v =>
                                v.AuthorId == UserName && v.ItemLink.Id == existingItem.Id);
                            if (existingVote == null)
                            {
                                context.Votes.Add(new Vote()
                                {
                                    AuthorId = UserName,
                                    AuthorName = User.Identity.Name,
                                    CreatedDate = DateTime.Now.ToUniversalTime(),
                                    DuplicateLink = true,
                                    ItemLink = existingItem,
                                });
                            }
                        }
                    }
                    else
                    {
                        addItem = new ItemLink
                        {
                            AuthorId = UserName,
                            AuthorName = User.Identity.Name,
                            CreatedDate = DateTime.Now.ToUniversalTime(),
                            Source = context.Items.Single(i => i.Id == value.SourceId),
                            Target = context.Items.Single(i => i.Id == value.TargetId),
                            RelationshipType = context.RelationshipTypes.Single(rt => rt.Id == value.RelationshipId),
                        };
                        context.ItemLinks.Add(addItem);
                    }
                }
                else
                {
                    var addConcept = new Item()
                    {
                        ItemType = ItemType.Concept,
                        Name = value.TargetName,
                        Description = "",
                        LastSyncTime = DateTime.Now.ToUniversalTime(),
                        Value = value.TargetName.ToLower().Replace("/", "_").Replace(" ", "_")
                    };
                    context.Items.Add(addConcept);
                    addItem = new ItemLink
                    {
                        AuthorId = UserName,
                        AuthorName = User.Identity.Name,
                        CreatedDate = DateTime.Now.ToUniversalTime(),
                        Source = context.Items.Single(i => i.Id == value.SourceId),
                        Target = addConcept,
                        RelationshipType = context.RelationshipTypes.Single(rt => rt.Id == value.RelationshipId)
                    };
                    context.ItemLinks.Add(addItem);
                }

                context.SaveChanges();
            }
        }

        // DELETE api/<StatementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (VastOntologyContext context = new VastOntologyContext())
            {
                var itemToRemove = context.ItemLinks.Single(i => i.Id == id && i.AuthorId == UserName);
                context.ItemLinks.Remove(itemToRemove);
                context.SaveChanges();
            }
        }
    }
}
