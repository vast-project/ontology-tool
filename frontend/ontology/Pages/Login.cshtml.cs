using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ontology.Pages
{
    [Authorize]
    public class LoginModel : PageModel
    {
        private static string _frontEndUrl = null;

        public LoginModel(IConfiguration configuration)
        {
            _frontEndUrl = configuration.GetValue<string>("FrontEndUrl");
        }

        public async void OnGet()
        {
            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                string accessToken = await HttpContext.GetTokenAsync("id_token");
                if (!string.IsNullOrWhiteSpace(accessToken)) {
                    Response.Redirect($"{_frontEndUrl}login-success/{accessToken}");
                }
            }
        }
    }
}
