using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalTeme.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portalteme.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase {

        public SettingsController() { }

        //[HttpGet("[action]")]
        //public async Task<ApplicationSettings> Get() {
        //    if (!User.Identity.IsAuthenticated)
        //        return new ApplicationSettings();

        //    var email = User.FindFirst("email");
        //    var firstName = User.FindFirst("firstName");
        //    var lastName = User.FindFirst("lastName");

        //    return new ApplicationSettings {
        //        User = new UserSettings {
        //            AuthToken = await HttpContext.GetTokenAsync("access_token"),
        //            Email = email?.Value,
        //            FirstName = firstName?.Value,
        //            LastName = lastName?.Value
        //        }
        //    };
        //}
    }

    //public class ApplicationSettings {
    //    public UserSettings User { get; set; }
    //}

    //public class UserSettings {
    //    public string Email { get; set; }
    //    public string AuthToken { get; set; }
    //    public string LastName { get; set; }
    //    public string FirstName { get; set; }
    //}
}
