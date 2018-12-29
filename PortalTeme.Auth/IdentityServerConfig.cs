using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using PortalTeme.Common.Authentication;
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
                new IdentityResource(AuthenticationConstants.RolesScope, new[] { JwtClaimTypes.Role }),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources() {
            return new List<ApiResource> {
                new ApiResource {
                    Name = AuthenticationConstants.ApplicationMainApi_Name,
                    ApiSecrets = {
                        new Secret("apisecret".Sha256())
                    },
                    DisplayName = "Portal Teme Main API",
                    Scopes = {
                        new Scope {
                            Name = AuthenticationConstants.ApplicationMainApi_FullAccessScope,
                            DisplayName = "Read/Write access to your data in the Portal Teme App"
                        },
                        new Scope {
                            Name = AuthenticationConstants.ApplicationMainApi_ReadOnlyScope,
                            DisplayName = "Read Only access to your data in the Portal Teme App"
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients() {
            return new List<Client> {
                new Client {
                    ClientId = AuthenticationConstants.AngularAppClientId,
                    ClientName = "Portal Teme WebApp",
                    ClientUri = AuthenticationConstants.AngularAppRootUrl,

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,

                    //TODO: enable when the UI is done
                    RequireConsent = false,

                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { AuthenticationConstants.AngularAppLoginCallback },
                    PostLogoutRedirectUris = { AuthenticationConstants.AngularAppLogoutCallback },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        AuthenticationConstants.RolesScope,
                        AuthenticationConstants.ApplicationMainApi_FullAccessScope,
                        AuthenticationConstants.ApplicationMainApi_ReadOnlyScope
                    },
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }

    }
}
