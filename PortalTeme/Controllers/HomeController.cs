using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
            return new AngularIndexViewModel { };
        }
    }
}