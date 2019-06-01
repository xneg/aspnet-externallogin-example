using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNet.ExternalLogin.Example.Controllers
{
    //без атрибута не работает, потому что внутри методы тоже используют атрибуты
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Route("signin")]
        public IActionResult SignIn() => View();

        [Route("signin/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl = null)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, provider);
        }
        
        [Route("signout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }
    }
}