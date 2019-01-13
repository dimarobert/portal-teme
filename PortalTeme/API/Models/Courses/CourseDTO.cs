using PortalTeme.API.Models.Assignments;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models.Courses {

    public class CourseEditDTO {
        public Guid? Id { get; set; }

        [Required]
        public CourseDefinitionRefDTO CourseDef { get; set; }

        [Required]
        public UserDTO Professor { get; set; }
    }

    public class CourseViewDTO : CourseEditDTO {

        public List<UserDTO> Assistants { get; set; }

        /// <summary>
        /// This defines the Study Groups that have access to the Course
        /// </summary>
        public List<CourseGroupDTO> Groups { get; set; }

        /// <summary>
        /// This defines the individual students that have access to the Course. (e.g. that have the course as an optional course)
        /// </summary>
        public List<UserDTO> Students { get; set; }


        public List<AssignmentDTO> Assignments { get; set; }
    }

    public class CourseRefDTO {

        [Required]
        public Guid Id { get; set; }
    }
}
