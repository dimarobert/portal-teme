using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models {

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
        public List<GroupRefDTO> Groups { get; set; }

        /// <summary>
        /// This defines the individual students that have access to the Course. (e.g. that have the course as an optional course)
        /// </summary>
        public List<UserDTO> Students { get; set; }


        //public List<Assignment> Assignments { get; set; }
    }

    public class CourseDefinitionRefDTO {

        [Required]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class UserDTO {

        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

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

}
