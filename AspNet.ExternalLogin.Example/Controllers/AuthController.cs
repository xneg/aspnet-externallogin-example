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
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        
        [Route("signout")]
        public async Task<IActionResult> SignOut()
        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            
            //var user = new IdentityUser("testUser");
            var tryLogin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

            if (!tryLogin.Succeeded)
            {
//                var user = await _userManager.Users.FirstAsync(u => u.UserName == "xneg");
                var user = new IdentityUser
                {
                    UserName = info.Principal.Identity.Name
                };
                await _userManager.CreateAsync(user);

                var x = await _userManager.AddLoginAsync(user, info);
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
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
//            if (remoteError != null)
//            {
//                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
//                return View(nameof(Login));
//            }
//            if (info == null)
//            {
//                return RedirectToAction(nameof(Login));
//            }
//
//            // Sign in the user with this external login provider if the user already has a login.
//            if (result.Succeeded)
//            {
//                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
//                return RedirectToLocal(returnUrl);
//            }
//            if (result.RequiresTwoFactor)
//            {
//                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
//            }
//            if (result.IsLockedOut)
//            {
//                return View("Lockout");
//            }
//            else
//            {
//                // If the user does not have an account, then ask the user to create an account.
//                ViewData["ReturnUrl"] = returnUrl;
//                ViewData["LoginProvider"] = info.LoginProvider;
//                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
//                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
//            }
        }

    }
}