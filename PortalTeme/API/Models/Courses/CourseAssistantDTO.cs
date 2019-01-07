using System;

namespace PortalTeme.API.Models.Courses {
    public class CourseAssistantDTO {

        public Guid CourseId { get; set; }

        public UserDTO Assistant { get; set; }
    }
}
