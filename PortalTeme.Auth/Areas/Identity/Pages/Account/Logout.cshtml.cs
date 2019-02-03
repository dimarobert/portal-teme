using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PortalTeme.Data.Identity;

namespace PortalTeme.Auth.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        private readonly IIdentityServerInteractionService interactionService;

        public LogoutModel(SignInManager<User> signInManager, IIdentityServerInteractionService interactionService, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            this.interactionService = interactionService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet(string logoutId)
        {
            var vm = await BuildLoggedOutViewModelAsync(logoutId);

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (vm.TriggerExternalSignout) {
                string url = Url.Page("Logout", new { logoutId = vm.LogoutId });
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return Redirect(vm.PostLogoutRedirectUri);
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return Page();
            }
        }


        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId) {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await interactionService.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true) {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider) {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout) {
                        if (vm.LogoutId == null) {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await interactionService.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }

    public class LoggedOutViewModel {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }

        public string LogoutId { get; set; }
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
        public string ExternalAuthenticationScheme { get; set; }
    }
}