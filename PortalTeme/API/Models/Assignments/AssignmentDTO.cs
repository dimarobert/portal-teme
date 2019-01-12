using PortalTeme.API.Models.Courses;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
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

    public class AssignmentEntryDTO : AssignmentEntryProjectionBase {
        public List<AssignmentEntryVersionDTO> Versions { get; set; }
    }

    public class AssignmentEntryVersionDTO {

        public Guid? Id { get; set; }

        public DateTime DateAdded { get; set; }

        public List<AssignmentEntryFileDTO> Files { get; set; }
    }

    public class AssignmentEntryFileDTO {

        public Guid? Id { get; set; }

        public AssignmentEntryFileType FileType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
