using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Application;

namespace PortalTeme.Data.Identity {
    public class IdentityContext : IdentityDbContext<User> {
        public IdentityContext(DbContextOptions options)
            : base(options) {
        }

        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
