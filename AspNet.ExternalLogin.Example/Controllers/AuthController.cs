using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        
        [Route("signout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var tryLogin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

            if (!tryLogin.Succeeded)
            {
                var user = new IdentityUser
                {
                    UserName = info.Principal.Identity.Name,
                };
                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "admin");
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            }

            if (returnUrl != null) 
                return Redirect(returnUrl);
            
//            {
//                // Store the access token and resign in so the token is included in the cookie
//                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
//                var props = new AuthenticationProperties();
//                props.StoreTokens(info.AuthenticationTokens);
//                await _signInManager.SignInAsync(user, props, info.LoginProvider);
//            }
            return Ok();
        }

        [HttpPost]
        [Route("setuserrole")]
        public async Task<IActionResult> SetUserRole(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) 
                return BadRequest();
            
            if (await _userManager.IsInRoleAsync(user, role))
                return Ok();

            await _userManager.AddToRoleAsync(user, role);

            return Ok();
        }
    }
}