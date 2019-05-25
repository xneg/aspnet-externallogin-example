using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNet.ExternalLogin.Example.Controllers
{
    public class ProtectedController : Controller
    {
        [Authorize(AuthenticationSchemes = "Identity.Application", Roles = "Admin")]
        public JsonResult Index()
        {
            return new JsonResult(new {result = "success"});
        }
    }
}