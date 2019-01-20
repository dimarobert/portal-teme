using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Data.Models.Assignments.Projections {
    public class AssignmentEntryProjectionBase {
        public Guid? Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid AssignmentTaskId { get; set; }

        public string StudentId { get; set; }

        public StudentAssignedTaskState State { get; set; }

        public int? Grading { get; set; }

    }

    public class AssignmentEntryProjection : AssignmentEntryProjectionBase {
        public List<TaskSubmission> Versions { get; set; }
    }
}
