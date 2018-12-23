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
            optionsBuilder.UseSqlServer(@"Server=.;Database=portal_teme;Trusted_Connection=True;MultipleActiveResultSets=true;",
                options => {
                    options.MigrationsAssembly("PortalTeme.Data");
                }
            );
        }



        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

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
