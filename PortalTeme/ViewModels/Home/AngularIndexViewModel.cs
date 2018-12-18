using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.ViewModels.Home {
    public class AngularIndexViewModel {

        public UserSettings User { get; set; }
    }

    public class UserSettings {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
