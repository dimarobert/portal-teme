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

    public class HomeController : Controller {

        public IActionResult AngularIndex() {
            return View(CreateModel());
        }

        public IActionResult DevAngularIndex() {
            return View(CreateModel());
        }

       

        private AngularIndexViewModel CreateModel() {

            if (!User.Identity.IsAuthenticated)
                return new AngularIndexViewModel();

            return new AngularIndexViewModel {
                User = new UserSettings {
                    Email = User.FindFirst(UserProfileConstants.EmailClaim)?.Value,
                    FirstName = User.FindFirst(UserProfileConstants.GivenNameClaim)?.Value,
                    LastName = User.FindFirst(UserProfileConstants.FamilyNameClaim)?.Value,
                }
            };
        }
    }
}