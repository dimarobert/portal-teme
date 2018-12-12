using IdentityServer4;
using IdentityServer4.Models;
using PortalTeme.Common.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth {
    public class IdentityServerConfig {

        public static IEnumerable<IdentityResource> GetIdentityResources() {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources() {
            return new List<ApiResource> {
                new ApiResource{
                    Name = AuthorizationConstants.ApplicationMainApi_Name,
                    DisplayName = "Portal Teme Main API",
                    Scopes = {
                        new Scope {
                            Name = AuthorizationConstants.ApplicationMainApi_FullAccessScope,
                            DisplayName = "Full Access to Portal Teme Main API"
                        },
                        new Scope {
                            Name = AuthorizationConstants.ApplicationMainApi_ReadOnlyScope,
                            DisplayName = "Read Only access to Portal Teme Main API"
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients() {
            return new List<Client> {
                new Client {
                    ClientId = AuthorizationConstants.AngularAppClientId,
                    ClientName = "Portal Teme WebApp",
                    ClientUri = AuthorizationConstants.AngularAppRootUrl,

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,

                    //TODO: enable when the UI is done
                    RequireConsent = false,

                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { AuthorizationConstants.AngularAppLoginCallback },
                    PostLogoutRedirectUris = { AuthorizationConstants.AngularAppLogoutCallback },
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        AuthorizationConstants.ApplicationMainApi_FullAccessScope,
                        AuthorizationConstants.ApplicationMainApi_ReadOnlyScope
                    }
                }
            };
        }

    }
}
