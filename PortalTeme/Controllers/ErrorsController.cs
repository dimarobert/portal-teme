using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Controllers {
    public class ErrorsController : Controller {
        public IActionResult Error(string message = null) {
            ViewData["Message"] = message ?? "Unknown error";
            return View();
        }

        public IActionResult AccessDenied(string schema = null) {
            return View(new AccessDeniedViewModel {
                Schema = schema
            });
        }
    }

    public class AccessDeniedViewModel {
        public string Schema { get; set; }
    }
}