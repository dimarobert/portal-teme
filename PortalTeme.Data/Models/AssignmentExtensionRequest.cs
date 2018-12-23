using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class AssignmentExtensionRequest {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public AssignmentEntry AssignmentEntry { get; set; }

        [Required]
        public string Reason { get; set; }

        public bool Approved { get; set; }
    }
}
