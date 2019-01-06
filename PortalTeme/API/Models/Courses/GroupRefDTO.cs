using System;

namespace PortalTeme.API.Models.Courses {
    /// <summary>
    /// This is a read-only reference to a Course-Group relation.
    /// </summary>
    public class GroupRefDTO {

        public Guid CourseId { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; }
    }

}
