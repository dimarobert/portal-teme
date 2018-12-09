using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalTeme.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portalteme.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase {
        private readonly UserManager<User> userManager;

        public SettingsController(UserManager<User> userManager) {
            this.userManager = userManager;
        }

        [HttpGet("[action]")]
        public async Task<ApplicationSettings> Get() {
            if (!User.Identity.IsAuthenticated)
                return new ApplicationSettings();

            var user = await userManager.GetUserAsync(User);

            return new ApplicationSettings {
                User = new UserSettings {
                    AuthToken = await HttpContext.GetTokenAsync("access_token"),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };
        }
    }

    public class ApplicationSettings {
        public UserSettings User { get; set; }
    }

    public class UserSettings {
        public string Email { get; set; }
        public string AuthToken { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
