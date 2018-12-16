using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalTeme.Common.UserProfile;
using PortalTeme.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Controllers {

    [Authorize]
    public class HomeController : Controller {

        public IActionResult AngularIndex() {
            return View(CreateModel());
        }

        public async Task<IActionResult> DevAngularIndex() {
            return View(await CreateModel());
        }
        private async Task<AngularIndexViewModel> CreateModel() {

            if (!User.Identity.IsAuthenticated)
                return new AngularIndexViewModel();

            var token = await HttpContext.GetTokenAsync("access_token");
            return new AngularIndexViewModel {
                AccessToken = token,
                User = new UserSettings {
                    Email = User.FindFirst(UserProfileConstants.EmailClaim)?.Value,
                    FirstName = User.FindFirst(UserProfileConstants.GivenNameClaim)?.Value,
                    LastName = User.FindFirst(UserProfileConstants.FamilyNameClaim)?.Value,
                }
            };
        }
    }
}