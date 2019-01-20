using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class StudentAssignedTask {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

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
