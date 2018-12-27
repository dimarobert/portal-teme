using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Data.Migrations {
    public class MigrationsContext : IdentityContext {

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<AssignmentEntry> AssignmentEntries { get; set; }

        public DbSet<AssignmentExtensionRequest> AssignmentExtensionRequests { get; set; }



        public MigrationsContext(DbContextOptions<MigrationsContext> options) : base(options) { }

        public MigrationsContext() : base(new DbContextOptionsBuilder<MigrationsContext>().Options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(@"Data Source=epa.dnnsharp.com;Initial Catalog=portal_teme;user id=portal_teme;password=ptPass!",
                options => {
                    options.MigrationsAssembly("PortalTeme.Data");
                }
            );
        }



        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole { Name = "Professor", NormalizedName = "Professor".ToUpper() },
                new IdentityRole { Name = "Assistant", NormalizedName = "Assistant".ToUpper() },
                new IdentityRole { Name = "Student", NormalizedName = "Student".ToUpper() });

            builder.Entity<CourseAssistant>()
                .HasKey(ca => new { ca.CourseId, ca.AssistantId });
            builder.Entity<CourseAssistant>()
                .HasOne(ca => ca.Course)
                .WithMany(course => course.Assistants)
                .HasForeignKey(ca => ca.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CourseAssistant>()
                .HasOne(ca => ca.Assistant)
                .WithMany()
                .HasForeignKey(ca => ca.AssistantId);

            builder.Entity<AssignmentEntry>()
                .HasOne(ae => ae.Student)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
