using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class TaskSubmissionFile {
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public FileInfo File { get; set; }
        public Guid FileId { get; set; }

        [Required]
        public TaskSubmissionFileType FileType { get; set; }

        public string Description { get; set; }

        public TaskSubmission TaskSubmission { get; set; }
    }

    // This may be temporary
    public enum TaskSubmissionFileType {
        SourceCode,
        Project,
        Essay
    }
}
