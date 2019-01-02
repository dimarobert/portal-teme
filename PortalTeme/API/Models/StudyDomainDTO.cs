using System;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models {
    public class StudyDomainDTO {

        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
