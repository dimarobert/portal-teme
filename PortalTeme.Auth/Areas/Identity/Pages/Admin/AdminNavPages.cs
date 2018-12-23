using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin {

    public static class AdminNavPages {

        public const string UserRoles = "UserRoles";

        public static string UserRolesNavClass(ViewContext viewContext) => PageHelper.GetActivePageClass(viewContext, UserRoles);

    }
}
