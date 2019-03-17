using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class AssignmentTask {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Assignment Assignment { get; set; }
        public Guid AssignmentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public List<StudentAssignedTask> StudentsAssigned { get; set; }

        public AssignmentTask() {
            StudentsAssigned = new List<StudentAssignedTask>();
        }
    }
}
