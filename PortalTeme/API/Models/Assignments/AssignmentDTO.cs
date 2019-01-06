using PortalTeme.API.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.API.Models.Assignments {
    public class AssignmentDTO {

        public Guid? Id { get; set; }

        public CourseEditDTO Course { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime EndDate { get; set; }
    }
}
