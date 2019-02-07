using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace PortalTeme.Data.Models {
    public class Group {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public StudyDomain Domain { get; set; }
        public Guid DomainId { get; set; }

        [Required]
        public AcademicYear Year { get; set; }
        public Guid YearId { get; set; }
    }
}
