using Microsoft.AspNetCore.Identity;
using System;

namespace PortalTeme.Data.Identity {

    public class User : IdentityUser {

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
