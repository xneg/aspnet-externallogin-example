using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExternalAuth.Controllers
{
    public class ProtectedController : Controller
    {
//        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "Admin")]
//        [Authorize(Roles = "admin")]
//            [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
//        [Authorize(AuthenticationSchemes = "Identity.External")]
        [Authorize]
        public JsonResult Index()
        {
            return new JsonResult(new {result = "success"});
        }
    }
}