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


        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        public DbSet<AssignmentEntry> AssignmentEntries { get; set; }

        public DbSet<AssignmentExtensionRequest> AssignmentExtensionRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Assignment>().Property(a => a.DateAdded).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<Assignment>().Property(a => a.LastUpdated).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<Assignment>().Property(a => a.StartDate).HasConversion(new DateTimeValueConverter());
            modelBuilder.Entity<Assignment>().Property(a => a.EndDate).HasConversion(new DateTimeValueConverter());

            modelBuilder.Entity<AssignmentEntryVersion>().Property(a => a.DateAdded).HasConversion(new DateTimeValueConverter());
        }

    }

    public class DateTimeValueConverter : ValueConverter<DateTime, DateTime> {
        public DateTimeValueConverter(ConverterMappingHints mappingHints = null) 
            : base(normalizeDate, normalizeDate, mappingHints) {
        }

        static Expression<Func<DateTime, DateTime>> normalizeDate = date => date.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : date.ToUniversalTime();
    }
}
