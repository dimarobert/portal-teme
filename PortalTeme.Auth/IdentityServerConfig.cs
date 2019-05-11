using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
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

        public static IEnumerable<ApiResource> GetApiResources(IConfiguration configuration) {
            var mainApiConf = configuration.GetSection(AuthenticationConstants.ApplicationMainApi_Name);
            var secret = mainApiConf.GetValue<string>("ApiSecret");

            return new List<ApiResource> {
                new ApiResource {
                    Name = AuthenticationConstants.ApplicationMainApi_Name,
                    ApiSecrets = {
                        new Secret(secret.Sha256())
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

        public static IEnumerable<Client> GetClients(IConfiguration configuration) {

            yield return BuildClient(configuration, AuthenticationConstants.AngularAppClientId);

            var iisDevClient = BuildClient(configuration, "IISAngularClient");
            if (iisDevClient != null)
                yield return iisDevClient;
        }

        private static Client BuildClient(IConfiguration configuration, string clientId) {
            var clientConf = configuration.GetSection(clientId);
            if (!clientConf.Exists())
                return null;

            var rootUri = clientConf.GetValue<string>("AppUri");
            var secret = clientConf.GetValue<string>("Secret");

            return new Client {
                ClientId = clientId,
                ClientName = "Portal Teme WebApp",
                ClientUri = rootUri,

                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,

                //TODO: enable when the UI is done
                RequireConsent = false,

                ClientSecrets = {
                        new Secret(secret.Sha256())
                    },

                RedirectUris = { AuthenticationConstants.AngularAppLoginCallback(rootUri) },
                PostLogoutRedirectUris = { AuthenticationConstants.AngularAppLogoutCallback(rootUri) },

                AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        AuthenticationConstants.RolesScope,
                        AuthenticationConstants.ApplicationMainApi_FullAccessScope,
                        AuthenticationConstants.ApplicationMainApi_ReadOnlyScope
                    },
                AlwaysIncludeUserClaimsInIdToken = true
            };
        }
    }
}
