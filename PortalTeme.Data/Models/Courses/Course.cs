using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortalTeme.Data.Models {

    public class Course {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public CourseDefinition CourseInfo { get; set; }

        [Required]
        public User Professor { get; set; }

        public List<CourseAssistant> Assistants { get; set; }

        /// <summary>
        /// This defines the Study Groups that have access to the Course
        /// </summary>
        public List<CourseGroup> Groups { get; set; }

        /// <summary>
        /// This defines the individual students that have access to the Course. (e.g. that have the course as an optional course)
        /// </summary>
        public List<CourseStudent> Students { get; set; }


        public List<Assignment> Assignments { get; set; }

    }
}
