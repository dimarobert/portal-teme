using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.Data.Models {
    public class StudentAssignedTask {

        [Required]
        public AssignmentTask Task { get; set; }
        public Guid TaskId { get; set; }

        [Required]
        public StudentInfo Student { get; set; }
        public string StudentId { get; set; }

        [Required]
        public StudentAssignedTaskState State { get; set; }

        public int? Grading { get; set; }

        public List<TaskSubmission> Submissions { get; set; }

    }

    public enum StudentAssignedTaskState {
        Assigned,
        Submitted,
        Reviewed,
        Graded
    }
}
