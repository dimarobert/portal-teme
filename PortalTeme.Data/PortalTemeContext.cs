using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PortalTeme.Data {
    public class PortalTemeContext : DbContext {

        public PortalTemeContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<StudentInfo> Students { get; internal set; }

        public DbSet<AcademicYear> AcademicYears { get; set; }

        public DbSet<CourseDefinition> CourseDefinitions { get; set; }

        public DbSet<StudyDomain> StudyDomains { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<AssignmentEntry> AssignmentEntries { get; set; }

        public DbSet<AssignmentExtensionRequest> AssignmentExtensionRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("AspNetUsers");

            modelBuilder.Entity<Assignment>().Property(a => a.DateAdded).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<Assignment>().Property(a => a.LastUpdated).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<Assignment>().Property(a => a.StartDate).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<Assignment>().Property(a => a.EndDate).HasConversion(new DateTimeValueConverter());

            modelBuilder.Entity<AssignmentEntryVersion>().Property(a => a.DateAdded).HasConversion(new DateTimeValueConverter());

            modelBuilder.Entity<AssignmentExtensionRequest>().Property(a => a.DateCreated).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<AssignmentExtensionRequest>().Property(a => a.DateApproved).HasConversion(new DateTimeValueConverter());

            modelBuilder.Entity<CourseAssistant>().HasKey(ca => new { ca.CourseId, ca.AssistantId });
            modelBuilder.Entity<CourseGroup>().HasKey(ca => new { ca.CourseId, ca.GroupId });
            modelBuilder.Entity<CourseStudent>().HasKey(ca => new { ca.CourseId, ca.StudentId });
        }

    }

    public class DateTimeValueConverter : ValueConverter<DateTime, DateTime> {
        public DateTimeValueConverter(ConverterMappingHints mappingHints = null)
            : base(normalizeDate, normalizeDate, mappingHints) {
        }

        private static Expression<Func<DateTime, DateTime>> normalizeDate = date => date.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : date.ToUniversalTime();
    }
}
