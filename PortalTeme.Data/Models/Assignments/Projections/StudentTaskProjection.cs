using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Data.Models.Assignments.Projections {
    public class StudentTaskProjectionBase {

        public Guid CourseId { get; set; }

        public AssignmentTask Task { get; set; }

        public string StudentId { get; set; }

        public StudentAssignedTaskState State { get; set; }

        public int? Grading { get; set; }

    }

    public class StudentTaskProjection : StudentTaskProjectionBase {
        public List<TaskSubmission> Submissions { get; set; }
    }
}
