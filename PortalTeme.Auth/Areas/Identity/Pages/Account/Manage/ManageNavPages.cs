using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace PortalTeme.Auth.Areas.Identity.Pages.Account.Manage {
    public static class ManageNavPages {
        public static string Index => "Index";

        public static string ChangePassword => "ChangePassword";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        public static string IndexNavClass(ViewContext viewContext) => PageHelper.GetActivePageClass(viewContext, Index);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageHelper.GetActivePageClass(viewContext, ChangePassword);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageHelper.GetActivePageClass(viewContext, ExternalLogins);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageHelper.GetActivePageClass(viewContext, PersonalData);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageHelper.GetActivePageClass(viewContext, TwoFactorAuthentication);


    }
}