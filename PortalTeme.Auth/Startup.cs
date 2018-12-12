using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortalTeme.Data;
using System;
using System.Collections.Generic;
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

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("IdentityContextConnection"),
                    sqlOpts => sqlOpts.MigrationsAssembly("PortalTeme.Auth")
                )
            );

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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
                .AddAspNetIdentity<User>();

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
