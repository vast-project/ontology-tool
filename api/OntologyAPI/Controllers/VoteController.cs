using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

namespace OntologyAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ontology")]
    [ApiController]
    public class VoteController : ControllerBase
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

        [HttpPost()]
        public void Post([FromBody] ItemLink value, bool? negative = false)
        {
            var voteNegative = false;
            if (negative == true)
            {
                voteNegative = true;
            }
            using (VastOntologyContext context = new VastOntologyContext())
            {
                var existingItem = context.ItemLinks.FirstOrDefault(il => il.Source.Id == value.Source.Id && il.Target.Id == value.Target.Id && il.RelationshipType.Id == value.RelationshipType.Id);
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
                                Negative = voteNegative
                            });
                        } else if (existingVote.Negative != voteNegative)
                        {
                            existingVote.Negative = voteNegative;
                        }
                    }
                }
                
                context.SaveChanges();
            }
        }

    }
}
