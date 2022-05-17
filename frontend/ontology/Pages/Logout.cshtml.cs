using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ontology.Pages
{
    public class LogoutModel : PageModel
    {
        private static string _frontEndUrl = null;

        public LogoutModel(IConfiguration configuration)
        {
            _frontEndUrl = configuration.GetValue<string>("FrontEndUrl");
        }

        public void OnGet()
        {
            HttpContext.SignOutAsync();
            Response.Redirect($"{_frontEndUrl}");
        }
    }
}
