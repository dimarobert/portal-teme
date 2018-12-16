using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using PortalTeme.Authorization;
using PortalTeme.Routing;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.Authority = AuthorizationConstants.AuthorityUri;
                options.RequireHttpsMetadata = false;

                options.ClientId = Common.Authentication.AuthenticationConstants.AngularAppClientId;
                options.ClientSecret = AuthorizationConstants.ClientSecret;
                options.ResponseType = "code id_token";

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                options.Scope.Add(IdentityServerConstants.StandardScopes.Email);
                options.Scope.Add(Common.Authentication.AuthenticationConstants.ApplicationMainApi_FullAccessScope);
                options.Scope.Add(Common.Authentication.AuthenticationConstants.ApplicationMainApi_ReadOnlyScope);

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
            });

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

                //routes.MapRoute(
                //    name: "identityServer",
                //    template: "signout-oidc",
                //    defaults: new { controller = "AuthProvider", action = "SignOut" });

                routes.MapRoute(
                    name: "error",
                    template: "/Error",
                    defaults: new { controller = "Errors", action = "Error" });

                routes.MapRoute(
                    name: "access-denied",
                    template: "/AccessDenied",
                    defaults: new { controller = "Errors", action = "AccessDenied" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "areasDefault",
                    template: "{area:exists}/{controller}/{action=Index}/{id?}");

                var angularIndexAction = Env.IsDevelopment() ? "DevAngularIndex" : "AngularIndex";
                routes.MapSpaWithWdsRoute(
                    name: "AngularSpa",
                    isDev: Env.IsDevelopment(),
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
    }
}
