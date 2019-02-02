using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class TaskSubmission {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public StudentAssignedTask AssignedTask { get; set; }
        public Guid AssignedTaskId { get; set; }

        public List<TaskSubmissionFile> Files { get; set; }

        public DateTime DateAdded { get; set; }

        public string Description { get; set; }

        [Required]
        public TaskSubmissionState State { get; set; }

        public string Review { get; set; }

        public int? Grading { get; set; }

        public TaskSubmission() {
            Files = new List<TaskSubmissionFile>();
        }
    }

    public enum TaskSubmissionState {
        Submitted,
        Reviewed,
        Graded
    }
}
