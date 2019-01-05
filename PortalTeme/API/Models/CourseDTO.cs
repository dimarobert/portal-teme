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
        public ProfessorDTO Professor { get; set; }

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

    public class ProfessorDTO {
        [Required(AllowEmptyStrings = false)]
        public string ProfessorId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class AssistantDTO {
        [Required]
        public Guid CourseId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string AssistantId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    /// <summary>
    /// This is a read-only reference to a Course-Group relation.
    /// </summary>
    public class GroupRefDTO {

        public Guid CourseId { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; }
    }

    public class StudentDTO {
        [Required]
        public Guid CourseId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

}
