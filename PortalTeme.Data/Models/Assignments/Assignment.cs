using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalTeme.Data.Models {
    public class Assignment {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Course Course { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        public AssignmentType Type { get; set; }

        public int NumberOfDuplicates { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<AssignmentTask> AssignmentTasks { get; set; }

    }

    public enum AssignmentType {
        SingleTask,
        SingleChoiceList,
        MultipleChoiceList,
        CustomAssignedTasks
    }
}
