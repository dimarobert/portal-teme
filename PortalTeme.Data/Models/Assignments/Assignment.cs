using PortalTeme.Data.Identity;
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

        public List<AssignmentVariant> AssignmentVariants { get; set; }

    }

    public enum AssignmentType {
        SingleHomework,
        SingleChoiceList,
        MultipleChoiceList,
        CustomAssignedHomework
    }

    public class AssignmentVariant {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Assignment Assignment { get; set; }
        public string AssignmentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public User Student { get; set; }
        public string StudentId { get; set; }
    }
}
