using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class AssignmentEntryVersion {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public AssignmentEntry AssignmentEntry { get; set; }

        public List<AssignmentEntryFile> Files { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
