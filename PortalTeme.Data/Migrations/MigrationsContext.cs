using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Common.Authorization;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Data.Migrations {
    public class MigrationsContext : IdentityContext {

        public DbSet<StudentInfo> Students { get; set; }

        public DbSet<AcademicYear> AcademicYears { get; set; }

        public DbSet<CourseDefinition> CourseDefinitions { get; set; }

        public DbSet<StudyDomain> StudyDomains { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<AssignmentEntry> AssignmentEntries { get; set; }

        public DbSet<AssignmentExtensionRequest> AssignmentExtensionRequests { get; set; }



        public MigrationsContext(DbContextOptions<IdentityContext> options) : base(options) { }

        public MigrationsContext() : base(new DbContextOptionsBuilder<IdentityContext>().Options) { }

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
                new IdentityRole { Id = "36f16a57-5b33-4bc3-99e8-51d61595ec2f", ConcurrencyStamp = "cf3a5c72-0813-415b-879b-685eb7942e64", Name = AuthorizationConstants.AdministratorRoleName, NormalizedName = AuthorizationConstants.AdministratorRoleName.ToUpper() },
                new IdentityRole { Id = "ea74f168-a8e8-4267-a078-a1c2d6ef2251", ConcurrencyStamp = "7706b80b-e1fc-4b06-870c-2a02a72b5684", Name = "Professor", NormalizedName = "Professor".ToUpper() },
                new IdentityRole { Id = "bd9d8efc-b46d-40e4-b0e2-5ce581d2bd0b", ConcurrencyStamp = "1969f591-fd47-409f-867e-0d0fdec0c584", Name = "Assistant", NormalizedName = "Assistant".ToUpper() },
                new IdentityRole { Id = "a42fcfeb-29d5-4f8e-9c31-a174b4388e02", ConcurrencyStamp = "39b8570f-c1ba-4e7b-9c64-49c0d15bf96f", Name = "Student", NormalizedName = "Student".ToUpper() });


            builder.Entity<StudentInfo>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<StudentInfo>(s => s.UserId);

            AddCourseAssistant_ManyToMany(builder);
            AddCourseGroup_ManyToMany(builder);
            AddCourseStudent_ManyToMany(builder);

            builder.Entity<AssignmentEntry>()
                .HasOne(ae => ae.Student)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

        }

        private void AddCourseAssistant_ManyToMany(ModelBuilder builder) {
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
        }

        private void AddCourseGroup_ManyToMany(ModelBuilder builder) {
            builder.Entity<CourseGroup>()
                .HasKey(cg => new { cg.CourseId, cg.GroupId });
            builder.Entity<CourseGroup>()
                .HasOne(cg => cg.Course)
                .WithMany(course => course.Groups)
                .HasForeignKey(cg => cg.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CourseGroup>()
                .HasOne(cg => cg.Group)
                .WithMany()
                .HasForeignKey(cg => cg.GroupId);
        }

        private void AddCourseStudent_ManyToMany(ModelBuilder builder) {
            builder.Entity<CourseStudent>()
                .HasKey(cs => new { cs.CourseId, cs.StudentId });
            builder.Entity<CourseStudent>()
                .HasOne(cs => cs.Course)
                .WithMany(course => course.Students)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CourseStudent>()
                .HasOne(cs => cs.Student)
                .WithMany()
                .HasForeignKey(cs => cs.StudentId);
        }

    }
}
