using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PortalTeme.Common.Authentication;

namespace PortalTeme.Auth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;

        public IndexModel(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public string AngularAppUri => configuration.GetSection(AuthenticationConstants.AngularAppClientId).GetValue<string>("AppUri");

        public void OnGet()
        {
        }
    }
}