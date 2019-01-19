using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class CourseDefinition {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public AcademicYear Year { get; set; }

        [Required]
        public Semester Semester { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Acronym { get; set; }

        public string Slug { get; set; }
    }

    public enum Semester {
        First = 0,
        Second = 1,
    }
}
