using Microsoft.AspNetCore.Authorization;

namespace OntologyAPI.Auth
{
// HasScopeHandler.cs

    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "realm" && c.Issuer == requirement.Issuer && c.Value=="/VAST_Tools"))
                return Task.CompletedTask;

            // Split the scopes string into an array
            //var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer)?.Value.Split(' ');

            // Succeed if the scope array contains the required scope
            //if ((scopes ?? Array.Empty<string>()).Any(s => s == requirement.Scope))
            
            //It is enough for now that the token is issued by the correct authority within the correct realm to authorize the use of the ontology tool
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
