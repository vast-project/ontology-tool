using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public int ContextId { get; set; } = 0;
    }

    public class VoteData
    {
        public int StatementId { get; set; }
        public bool Negative { get; set; }
    }

    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ontology")]
    [ApiController]
    public class StatementController : ControllerBase
    {
        private VastOntologyContext _ontologyContext = null;
        public StatementController(VastOntologyContext ontologyContext)
        {
            _ontologyContext = ontologyContext;
        }

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
        public IEnumerable<object> GetAll(string? search = null, int? sourceId = null, int? targetId = null, int context = 0)
        {

            {
                IQueryable<ItemLink> items = _ontologyContext.ItemLinks;

                //If a context is provided, filter by the context
                if (context > 0)
                {
                    items = items.Where(i => i.PrimaryContext.Id == context);
                }

                return items.Select(i => new
                {
                    i.Id,
                    i.AuthorId,
                    i.AuthorName,
                    Source = new { i.Source.Id, i.Source.ItemType, i.Source.Name },
                    Target = new { i.Target.Id, i.Target.ItemType, i.Target.Name },
                    RelationshipType = new { i.RelationshipType.Name, i.RelationshipType.OntologyId, i.RelationshipType.Id },
                }).ToList();
            }
        }

        [HttpGet("me")]
        public IEnumerable<object> GetMine(string? search = null, int? sourceId = null, int? targetId = null, int context = 0)
        {

            {
                IQueryable<ItemLink> items = _ontologyContext.ItemLinks.Where(i => i.AuthorId == UserName || i.Votes.Any(v => v.AuthorId == UserName && v.DuplicateLink == true));
                //If a context is provided, filter by the context
                if (context > 0)
                {
                    items = items.Where(i => i.PrimaryContext.Id == context);
                }
                else
                {
                    items = items.Where(c => c.PrimaryContext.Id != -1);
                }

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
        public IEnumerable<object> GetOthers(string? search = null, int? sourceId = null, int? targetId = null, int? linkTypeId = null, int context = 0)
        {

            {
                IQueryable<ItemLink> items = _ontologyContext.ItemLinks.Where(i => i.AuthorId != UserName);

                //If a context is provided, filter by the context
                if (context > 0)
                {
                    items = items.Where(i => i.PrimaryContext.Id == context);
                }
                else
                {
                    items = items.Where(c => c.PrimaryContext.Id != -1);
                }

                if (!String.IsNullOrWhiteSpace(search))
                {
                    items = items.Where(i =>
                        i.Source.Name.ToLower().Contains(search) || i.Target.Name.ToLower().Contains(search));
                }
                if (sourceId != null)
                {
                    items = items.Where(i => i.Source.Id == sourceId);
                }
                if (targetId != null)
                {
                    items = items.Where(i => i.Target.Id == targetId);
                }
                if (linkTypeId != null)
                {
                    items = items.Where(i => i.RelationshipType.Id == linkTypeId);
                }

                return items.Select(i => new
                {
                    i.Id,
                    i.AuthorId,
                    i.AuthorName,
                    Source = new { i.Source.Id, i.Source.ItemType, i.Source.Name },
                    Target = new { i.Target.Id, i.Target.ItemType, i.Target.Name },
                    RelationshipType = new { i.RelationshipType.Name, i.RelationshipType.OntologyId, i.RelationshipType.Id },
                    Voted = i.Votes.Any(v => v.AuthorId == UserName),
                    Votes = i.Votes.Where(v => !v.Negative).Count() - i.Votes.Where(v => v.Negative).Count(),
                    VotedUp = i.Votes.Any(v => v.AuthorId == UserName && v.Negative == false),
                    VotedDown = i.Votes.Any(v => v.AuthorId == UserName && v.Negative == true)
                }).ToList();
            }
        }

        [HttpGet("rel-types")]
        public IEnumerable<object> GetRelType(string? search = null, int context = 0)
        {

            {
                IQueryable<RelationshipType> items = _ontologyContext.RelationshipTypes;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    items = items.Where(x => x.Name.ToLower().Contains(search.ToLower()));
                }

                if (context >= 2 && context <= 4)
                {
                    items = items.Where(x => x.Id == 4);
                }

                var results = items.Select(i => new { i.Id, i.Name, i.OntologyId });
                return results.ToList();
            }
        }

        [HttpPost("vote")]
        public void Vote(VoteData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            int id = data.StatementId;
            bool negative = data.Negative;

            if (id <= 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var existingItem = _ontologyContext.ItemLinks.FirstOrDefault(il => il.Id == id);
            if (existingItem == null || existingItem.AuthorId == UserName)
            {
                throw new ArgumentException("The provided id does not exist or belongs to the user voting.");
            }

            var existingVote = _ontologyContext.Votes.SingleOrDefault(v => v.ItemLink.Id == existingItem.Id && v.AuthorId == UserName);
            if (existingVote == null)
            {
                _ontologyContext.Votes.Add(new Vote
                {
                    Negative = negative,
                    AuthorId = UserName,
                    AuthorName = User?.Identity?.Name ?? UserName,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    ItemLink = existingItem
                });
            }
            else
            {
                existingVote.Negative = negative;
                existingVote.CreatedDate = DateTime.Now.ToUniversalTime();
            }

            _ontologyContext.SaveChanges();
        }

        // PUT api/<StatementController>/5
        [HttpPost()]
        public void Post([FromBody] StatementData value)
        {
            if (value == null || (value.TargetId == null && String.IsNullOrWhiteSpace(value.TargetName)))
                throw new ArgumentNullException(nameof(value));


            ItemLink addItem;
            if (value.TargetId != null)
            {
                var existingItem = _ontologyContext.ItemLinks.FirstOrDefault(il => il.Source.Id == value.SourceId && il.Target.Id == value.TargetId && il.RelationshipType.Id == value.RelationshipId && il.PrimaryContext.Id == value.ContextId && il.AuthorId == UserName);
                if (existingItem != null)
                {
                    if (existingItem.AuthorId == UserName)
                    {
                        throw new ArgumentException("Statement already exists.");
                    }
                }
                else
                {
                    addItem = new ItemLink
                    {
                        AuthorId = UserName,
                        AuthorName = User?.Identity?.Name ?? UserName,
                        CreatedDate = DateTime.Now.ToUniversalTime(),
                        Source = _ontologyContext.Items.Single(i => i.Id == value.SourceId),
                        Target = _ontologyContext.Items.Single(i => i.Id == value.TargetId),
                        RelationshipType = _ontologyContext.RelationshipTypes.Single(rt => rt.Id == value.RelationshipId),
                        PrimaryContext = value.ContextId <= 0 ? null : _ontologyContext.Contexts.Single(c => c.Id == value.ContextId)
                    };
                    _ontologyContext.ItemLinks.Add(addItem);
                }
            }
            else
            {
                Item addConcept = _ontologyContext.Items.Where(i =>
                    i.Name.Trim().ToLower() == value.TargetName.ToLower().Trim() && i.ItemType == ItemType.Concept).Include(c => c.Contexts).FirstOrDefault();
                if (addConcept == null)
                {
                    addConcept = new Item()
                    {
                        ItemType = ItemType.Concept,
                        Name = value.TargetName.Trim(),
                        Description = "",
                        LastSyncTime = DateTime.Now.ToUniversalTime(),
                        Value = value.TargetName.ToLower().Replace("/", "_").Replace(" ", "_"),
                        PrimaryContext = value.ContextId <= 0 ? null : _ontologyContext.Contexts.Single(c => c.Id == value.ContextId)
                    };
                    _ontologyContext.Items.Add(addConcept);
                }
                else
                {
                    if (!_ontologyContext.Items.Any(i => i.Id == addConcept.Id && i.Contexts.Any(c => c.Id == value.ContextId)))
                    {
                        addConcept.Contexts.Add(_ontologyContext.Contexts.Single(c => c.Id == value.ContextId));
                    }
                }

                addItem = new ItemLink
                {
                    AuthorId = UserName,
                    AuthorName = User?.Identity?.Name ?? UserName,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    Source = _ontologyContext.Items.Single(i => i.Id == value.SourceId),
                    Target = addConcept,
                    RelationshipType = _ontologyContext.RelationshipTypes.Single(rt => rt.Id == value.RelationshipId),
                    PrimaryContext = value.ContextId <= 0 ? null : _ontologyContext.Contexts.Single(c => c.Id == value.ContextId),

                };
                _ontologyContext.ItemLinks.Add(addItem);
            }

            _ontologyContext.SaveChanges();

        }

        // DELETE api/<StatementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            try
            {
                var itemToRemove = _ontologyContext.ItemLinks.Single(i => i.Id == id && i.AuthorId == UserName);
                _ontologyContext.ItemLinks.Remove(itemToRemove);
                _ontologyContext.SaveChanges();
            }
            catch (Exception)
            {
                throw new AuthenticationException();
            }
        }
    }
}
