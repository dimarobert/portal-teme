using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Serialization;
using PortalTeme.API.Mappers;
using PortalTeme.Common.Authentication;
using PortalTeme.Common.Caching;
using PortalTeme.Data;
using PortalTeme.Data.Authorization.Policies;
using PortalTeme.Data.Identity;
using PortalTeme.Extensions.CacheExtensions;
using PortalTeme.HostedServices;
using PortalTeme.Routing;
using PortalTeme.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalTeme {
    public class Startup {

        public Startup(IHostingEnvironment env, IConfiguration configuration) {
            Env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddAntiforgery();

            services.AddSingleton<IJsonSerializer, JsonNetSerializer>();

            services.AddSingleton<IFileProvider, ContentRootFileProvider>();
            services.AddHostedService<TempFilesCleaner>();

            // TODO: This could be registered as singleton
            services.AddDbContext<FilesContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FilesContextConnection"))
            );
            services.AddDbContext<PortalTemeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PortalTemeContextConnection"))
            );
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityContextConnection"))
            );

            services.AddIdentityCore<User>(options => {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

            services.AddSingleton<IUrlSlugService, UrlSlugService>();
            services.AddSingleton<ITempFilesRepository, InMemoryTempFilesRepository>();

            services.AddScoped<IFileService, DbFileService>();

            services.AddScoped<ICourseMapper, CourseMapper>();
            services.AddScoped<IAssignmentMapper, AssignmentMapper>();
            services.AddScoped<ITaskMapper, TaskMapper>();

            // TODO: Update to use Redis (at least in prod)
            services.AddDistributedMemoryCache();
            services.Replace(ServiceDescriptor.Singleton<IDistributedCache, ExtendedMemoryDistributedCache>());
            services.AddSingleton<IExtendedDistributedCache, ExtendedMemoryDistributedCache>();

            services.AddSingleton<ICacheService, DistributedCacheService>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, SetupCookieSettings)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, SetupOpenIdSettings)
            .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, SetupIdServerAuth);

            services.AddAuthorization(SetupAuthorization);

            services.AddScoped<IAuthorizationHandler, AdminAuthorizatonHandler>();
            services.AddScoped<IAuthorizationHandler, CourseAuthorizatonCrudHandler>();
            services.AddScoped<IAuthorizationHandler, AssignmentAuthorizatonCrudHandler>();
            services.AddScoped<IAuthorizationHandler, StudentTasksAuthorizatonCrudHandler>();
            services.AddScoped<IAuthorizationHandler, GroupsAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, StudyDomainsAuthorizationHandler>();

            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = Path.Combine(Env.ContentRootPath, @"ClientApp\dist");
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app) {
            if (Env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes => {

                MapErrorsController(routes);
                MapDefaults(routes);

                var angularIndexAction = Env.IsDevelopment() ? "DevAngularIndex" : "AngularIndex";
                routes.MapSpaWithWdsRoute(
                    name: "AngularSpa",
                    defaults: new { controller = "Home", action = angularIndexAction });
            });

            app.UseSpa(spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = Path.Combine(Env.ContentRootPath, "ClientApp");

                if (Env.IsDevelopment()) {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }


        // ConfigureServices

        private void SetupCookieSettings(CookieAuthenticationOptions options) {
            options.Events = new CookieAuthenticationEvents {
                OnValidatePrincipal = RunRefreshTokenLogic
            };
        }

        private async Task RunRefreshTokenLogic(CookieValidatePrincipalContext context) {
            var authorizationSection = Configuration.GetSection("Authorization");


            if (!context.Principal.Identity.IsAuthenticated)
                return;

            var refreshToken = context.Properties.GetTokenValue(OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken is null)
                return;

            var exp = context.Properties.GetTokenValue("expires_at");
            var expires = DateTime.Parse(exp);
            if (expires > DateTime.Now.AddMinutes(-1))
                return;

            var disco = await DiscoveryClient.GetAsync(authorizationSection.GetValue<string>("AuthorityUri"));
            if (disco.IsError)
                return;

            var tokenClient = new TokenClient(disco.TokenEndpoint, authorizationSection.GetValue<string>("ClientId"), authorizationSection.GetValue<string>("ClientSecret"));
            var clientResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);

            if (clientResponse.IsError) {
                context.RejectPrincipal();
                return;
            }

            var newExpires = DateTime.UtcNow + TimeSpan.FromSeconds(clientResponse.ExpiresIn);

            context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, clientResponse.RefreshToken);
            context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, clientResponse.AccessToken);
            context.Properties.UpdateTokenValue("expires_at", newExpires.ToString("o", CultureInfo.InvariantCulture));

            //trigger context to renew cookie with new token values
            context.ShouldRenew = true;
        }


        private void SetupOpenIdSettings(OpenIdConnectOptions options) {
            var authorizationSection = Configuration.GetSection("Authorization");

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            options.Authority = authorizationSection.GetValue<string>("AuthorityUri");
            options.RequireHttpsMetadata = false;

            options.ClientId = authorizationSection.GetValue<string>("ClientId");
            options.ClientSecret = authorizationSection.GetValue<string>("ClientSecret");
            options.ResponseType = "code id_token";

            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.ClaimActions.MapJsonKey("role", "role", "role");

            options.Scope.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
            options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
            options.Scope.Add(IdentityServerConstants.StandardScopes.Email);
            options.Scope.Add(AuthenticationConstants.ApplicationMainApi_FullAccessScope);
            options.Scope.Add(AuthenticationConstants.ApplicationMainApi_ReadOnlyScope);
            options.Scope.Add(AuthenticationConstants.RolesScope);

            options.Events = new OpenIdConnectEvents {
                OnRemoteFailure = (ctx) => {
                    ctx.Response.Redirect("/AccessDenied?schema=oidc");
                    ctx.HandleResponse();
                    return Task.CompletedTask;
                },
                OnRedirectToIdentityProvider = ctx => {
                    var returnUrlValues = ctx.HttpContext.Request.Query["returnUrl"];
                    if (returnUrlValues.Any()) {
                        var returnUri = new Uri(returnUrlValues.First(), UriKind.RelativeOrAbsolute);
                        if (!returnUri.IsAbsoluteUri) {
                            var request = ctx.HttpContext.Request;
                            returnUri = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1, returnUri.OriginalString).Uri;
                        }
                        ctx.Properties.RedirectUri = returnUri.ToString();
                    }
                    return Task.CompletedTask;
                }
            };
        }

        private void SetupIdServerAuth(IdentityServerAuthenticationOptions options) {
            var authorizationSection = Configuration.GetSection("Authorization");

            options.Authority = authorizationSection.GetValue<string>("AuthorityUri");

            options.ApiName = AuthenticationConstants.ApplicationMainApi_Name;
            options.ApiSecret = authorizationSection.GetValue<string>("MainApiSecret");

            options.EnableCaching = true;
            options.CacheDuration = TimeSpan.FromMinutes(10);

            options.RequireHttpsMetadata = false;

            options.JwtBearerEvents = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents {
                OnTokenValidated = async context => {

                    if (!context.Principal.Identity.IsAuthenticated)
                        return;

                    var identity = context.Principal.Identity as ClaimsIdentity;
                    if (identity is null)
                        return;

                    var accessToken = context.SecurityToken as JwtSecurityToken;

                    var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                    var cachedClaims = await cache.GetClaimsAsync(accessToken.RawData);

                    if (cachedClaims is null) {

                        var discoveryClient = new DiscoveryClient(context.Options.Authority);
                        discoveryClient.Policy.RequireHttps = false; // TODO: Reenable https
                        var doc = await discoveryClient.GetAsync();
                        var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);
                        var response = await userInfoClient.GetAsync(accessToken.RawData);

                        if (response.IsError)
                            return;

                        var loggerFact = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFact.CreateLogger("Startup.OnTokenValidated");

                        var resultClaims = response.Claims;
                        var exp = identity.FindFirst(JwtClaimTypes.Expiration);
                        if (!(exp is null))
                            resultClaims = resultClaims.Append(exp);

                        await cache.SetClaimsAsync(accessToken.RawData, resultClaims, options.CacheDuration, logger);
                        cachedClaims = response.Claims;
                    }

                    var newClaims = cachedClaims.Where(claim => !identity.HasClaim(claim.Type, claim.Value));
                    identity.AddClaims(newClaims);

                }
            };
        }

        private void SetupAuthorization(AuthorizationOptions options) {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(Common.Authorization.AuthorizationConstants.AdministratorPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.RequireClaim("role", Common.Authorization.AuthorizationConstants.AdministratorRoleName);
            });

            AddCoursePolicies(options);
            AddGroupsPolicies(options);
            AddStudyDomainsPolicies(options);
            AddStudentTasksPolicies(options);
        }

        private void AddCoursePolicies(AuthorizationOptions options) {
            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanViewCoursePolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.Read);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanCreateCoursePolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.Create);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanUpdateCoursePolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.Update);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanEditCourseAssistantsPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.CourseEditAssistents);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanDeleteCoursePolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.Delete);
            });
        }

        private void AddGroupsPolicies(AuthorizationOptions options) {
            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanViewGroupsPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.ViewGroups);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanEditGroupsPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.EditGroups);
            });
        }

        private void AddStudyDomainsPolicies(AuthorizationOptions options) {
            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanViewStudyDomainsPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.ViewDomains);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanEditStudyDomainsPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.EditDomains);
            });
        }

        private void AddStudentTasksPolicies(AuthorizationOptions options) {
            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanViewStudentTaskPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.ViewStudentTask);
            });

            options.AddPolicy(Common.Authorization.AuthorizationConstants.CanEditStudentTaskPolicy, policy => {
                policy.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);

                policy.AddRequirements(Common.Authorization.Operations.EditStudentTask);
            });
        }

        // Configure

        private void MapErrorsController(Microsoft.AspNetCore.Routing.IRouteBuilder routes) {
            routes.MapRoute(
                name: "error",
                template: "/Error",
                defaults: new { controller = "Errors", action = "Error" });

            routes.MapRoute(
                name: "access-denied",
                template: "/AccessDenied",
                defaults: new { controller = "Errors", action = "AccessDenied" });
        }

        private void MapDefaults(Microsoft.AspNetCore.Routing.IRouteBuilder routes) {
            routes.MapRoute(
                name: "default",
                template: "{controller}/{action=Index}/{id?}");

            routes.MapRoute(
                name: "areasDefault",
                template: "{area:exists}/{controller}/{action=Index}/{id?}");
        }

    }
}
