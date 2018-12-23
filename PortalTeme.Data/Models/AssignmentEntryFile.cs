using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class AssignmentEntryFile {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public AssignmentEntryFileType FileType { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    // This may be temporary
    public enum AssignmentEntryFileType {
        SourceCode,
        Project,
        Essay
    }
}
