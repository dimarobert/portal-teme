using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models {
    public class CourseDTO {
        public Guid? Id { get; set; }

        [Required]
        public Guid CourseDef { get; set; }

        [Required]
        public string Professor { get; set; }

        public List<AssistantDTO> Assistants { get; set; }

        /// <summary>
        /// This defines the Study Groups that have access to the Course
        /// </summary>
        public List<GroupRefDTO> Groups { get; set; }

        /// <summary>
        /// This defines the individual students that have access to the Course. (e.g. that have the course as an optional course)
        /// </summary>
        public List<StudentDTO> Students { get; set; }


        //public List<Assignment> Assignments { get; set; }
    }

    public class AssistantDTO {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public string AssistantId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class GroupRefDTO {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        public string Name { get; set; }
    }

    public class StudentDTO {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public string StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

}
