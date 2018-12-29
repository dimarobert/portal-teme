using System;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.Data.Models {
    public class CourseStudent {
        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public string StudentId { get; set; }
        public StudentInfo Student { get; set; }

    }
}
