using System;

namespace PortalTeme.API.Models.Courses {
    public class CourseStudentDTO {
        public Guid CourseId { get; set; }

        public UserDTO Student { get; set; }
    }
}
