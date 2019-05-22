using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ExternalAuth.Controllers
{
    public class AuthSchemesController : Controller
    {
        private readonly AuthenticationService _authenticationService;

        public AuthSchemesController(IAuthenticationService authenticationService)
        {
            _authenticationService = (AuthenticationService)authenticationService;
        }

        public async Task<JsonResult> Index()
        {
            var Schemes = await _authenticationService.Schemes.GetAllSchemesAsync();
            var DefaultAuthenticate = await _authenticationService.Schemes.GetDefaultAuthenticateSchemeAsync();
            var DefaultForbid = await _authenticationService.Schemes.GetDefaultForbidSchemeAsync();
            var DefaultSignIn = await _authenticationService.Schemes.GetDefaultSignInSchemeAsync();
            var DefaultSignOut = await _authenticationService.Schemes.GetDefaultSignOutSchemeAsync();
            var DefaultChallenge = await _authenticationService.Schemes.GetDefaultChallengeSchemeAsync();
            return new JsonResult(new
            {
                Schemes,
                DefaultAuthenticate,
                DefaultForbid,
                DefaultSignIn,
                DefaultSignOut,
                DefaultChallenge
            });
        }
    }
}