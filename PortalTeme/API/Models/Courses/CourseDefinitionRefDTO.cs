using System;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models.Courses {
    public class CourseDefinitionRefDTO {

        [Required]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

}
