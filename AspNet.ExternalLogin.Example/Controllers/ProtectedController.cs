using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExternalAuth.Controllers
{
    public class ProtectedController : Controller
    {
        [Authorize]
        public JsonResult Index()
        {
            return new JsonResult(new {result = "success"});
        }
    }
}