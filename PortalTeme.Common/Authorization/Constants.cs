using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Common.Authorization {
    public static class AuthorizationConstants {

        public const string AngularAppClientId = "PortalTemeAngularWebApp";

        public const string AngularAppRootUrl = "https://localhost:44327";
        public const string AngularAppLoginCallback = AngularAppRootUrl + "/signin-oidc";
        public const string AngularAppLogoutCallback = AngularAppRootUrl + "/signout-callback-oidc";

        public const string ApplicationMainApi_Name = "PortalTemeApi";
        public const string ApplicationMainApi_FullAccessScope = "PortalTemeApi.FullAccess";
        public const string ApplicationMainApi_ReadOnlyScope = "PortalTemeApi.ReadOnly";
    }
}
