using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin {

    public static class AdminNavPages {

        public const string Users = "Users";

        public const string UserRoles = "UserRoles";
        public const string AddRole = "AddRole";
        public const string ViewUsersInRole = "ViewUsersInRole";
        public const string AssignRole = "AssignRole";

        public static string UserRolesNavClass(ViewContext viewContext)
            => PageHelper.GetActivePageClass(viewContext, UserRoles) ??
            PageHelper.GetActivePageClass(viewContext, ViewUsersInRole) ??
            PageHelper.GetActivePageClass(viewContext, AddRole) ??
            PageHelper.GetActivePageClass(viewContext, AssignRole);

        public static string UsersNavClass(ViewContext viewContext)
            => PageHelper.GetActivePageClass(viewContext, Users);

    }
}
