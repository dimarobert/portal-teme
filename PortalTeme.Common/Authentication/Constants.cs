using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Common.Authentication {
    public static class AuthenticationConstants {

        public const string AngularAppClientId = "PortalTemeAngularWebApp";

        public static string AngularAppLoginCallback(string rootUri) => rootUri + "/signin-oidc";
        public static string AngularAppLogoutCallback(string rootUri) => rootUri + "/signout-callback-oidc";

        public const string ApplicationMainApi_Name = "PortalTemeApi";

        public const string ApplicationMainApi_FullAccessScope = "PortalTemeApi.FullAccess";
        public const string ApplicationMainApi_ReadOnlyScope = "PortalTemeApi.ReadOnly";

        public const string RolesScope = "user_roles";
    }
}
