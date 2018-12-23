using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortalTeme.Auth.Areas.Identity;
using PortalTeme.Auth.Areas.Identity.Managers;
using PortalTeme.Auth.Areas.Identity.Stores;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddUserManager<ApplicationUserManager>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/Identity/Account/Login";
                    options.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
                })
                .AddDeveloperSigningCredential() // TODO: Replace with .AddSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients())
                .AddAspNetIdentity<User>()
                .AddProfileService<ProfileService>();

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
}
