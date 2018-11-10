using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portal_teme {
    public class IdentityServerConfig {

        public static IEnumerable<IdentityResource> GetIdentityResources() {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources() {
            return new List<ApiResource> {
                new ApiResource("api1", "My API")
            };
        }

        public static IEnumerable<Client> GetClients() {
            return new List<Client> {
                new Client {
                    ClientId = "angular spa",
                    ClientName = "Portal Teme WebApp",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    //TODO: enable when the UI is done
                    RequireConsent = false,

                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "https://localhost:44327/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44327/signout-callback-oidc" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                }
            };
        }

    }
}
