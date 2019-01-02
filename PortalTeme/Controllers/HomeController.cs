using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

            var userRoles = User.FindAll(IdentityModel.JwtClaimTypes.Role).Select(claim => claim.Value).Distinct().ToList();
            return new AngularIndexViewModel {
                User = new UserSettings {
                    Email = User.FindFirst(IdentityModel.JwtClaimTypes.Email)?.Value,
                    FirstName = User.FindFirst(IdentityModel.JwtClaimTypes.GivenName)?.Value,
                    LastName = User.FindFirst(IdentityModel.JwtClaimTypes.FamilyName)?.Value,
                    Roles = JsonConvert.SerializeObject(userRoles)
                }
            };
        }
    }
}