using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PortalTeme.Auth.Areas.Identity;
using PortalTeme.Auth.Areas.Identity.Managers;
using PortalTeme.Auth.Areas.Identity.Stores;
using PortalTeme.Auth.Authorization;
using PortalTeme.Auth.Services;
using PortalTeme.Common.Authorization;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PortalTeme.Auth {
    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddTransient<IIdentityLocalizer, IdentityLocalizer>();


            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityContextConnection"))
            );
            services.AddSingleton<AppInitialization>();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddUserManager<ApplicationUserManager>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddAuthorization(options => {
                options.AddPolicy("isInSetupMode", policy => policy.AddRequirements(new SetupModeRequirement()));

                options.AddPolicy(AuthorizationConstants.AdministratorPolicy, policy => policy.RequireRole(AuthorizationConstants.AdministratorRoleName));
            });

            services.AddSingleton<IAuthorizationHandler, SetupModeRequirementHandler>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options => {
                    options.Conventions.AuthorizeAreaFolder("Setup", "/", "isInSetupMode");

                    options.Conventions.AuthorizeAreaFolder("Identity", "/Admin/", AuthorizationConstants.AdministratorPolicy);
                });

            var developerTempCert = Path.Combine(Configuration[Microsoft.Extensions.Hosting.HostDefaults.ContentRootKey], "tempkey.rsa");

            services.AddIdentityServer(options => {
                options.UserInteraction.LoginUrl = "/Identity/Account/Login";
                options.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
            })
                .AddDeveloperSigningCredential(true, developerTempCert) // TODO: Replace with .AddSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources(Configuration))
                .AddInMemoryClients(IdentityServerConfig.GetClients(Configuration))
                .AddAspNetIdentity<User>()
                .AddProfileService<ProfileService>();


            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, ConfigureCookieOptions>();

            services.AddAuthentication(IdentityConstants.ApplicationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRequestLocalization(options => {

                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");

                options.SupportedCultures = options.SupportedUICultures = new[] {
                    new CultureInfo("en-US"),
                    new CultureInfo("ro-RO")
                };

            });

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.Use(async (context, next) => {
                var init = context.RequestServices.GetRequiredService<AppInitialization>();
                if (await init.IsInitialized()) {
                    await next();
                    return;
                }

                if (!context.Request.Path.StartsWithSegments("/setup")) {
                    var uri = new UriBuilder(context.Request.GetDisplayUrl()) {
                        Path = "/setup"
                    };
                    context.Response.Redirect(uri.Uri.ToString(), false);
                }

                await next();
            });

            app.UseMvc(routes => {

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}"
                );

                routes.MapRoute(
                    name: "areasDefault",
                    template: "{area:exists}/{controller}/{action=Index}/{id?}"
                );
            });
        }
    }

    public class ConfigureCookieOptions : IPostConfigureOptions<CookieAuthenticationOptions> {
        public void PostConfigure(string name, CookieAuthenticationOptions options) {
            options.AccessDeniedPath = "/AccessDenied";
        }
    }
}
