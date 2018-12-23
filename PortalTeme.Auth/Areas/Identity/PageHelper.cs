using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity {
    public static class PageHelper {

        public static bool IsActivePage(ViewContext viewContext, string page) {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase);
        }


        public static string GetActivePageClass(ViewContext viewContext, string page) {
            return IsActivePage(viewContext, page) ? "active" : null;
        }
    }
}
