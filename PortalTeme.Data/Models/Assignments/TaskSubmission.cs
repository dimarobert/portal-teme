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

        public List<TaskSubmissionFile> Files { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
