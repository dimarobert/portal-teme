using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Controllers {

    [Route("[controller]")]
    public class AuthenticationController : Controller {

        public AuthenticationController() { }

        [HttpGet("[action]")]
        public IActionResult Login(string returnUri = null) {
            if (User.Identity.IsAuthenticated)
                return Redirect(returnUri ?? "/");

            return Challenge(new AuthenticationProperties {
                RedirectUri = returnUri
            });
        }

        [HttpGet("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Token() {
            var token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            HttpContext.Response.Headers.Add("AccessToken", token);

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}