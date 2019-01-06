using System;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models.Courses {
    public class StudyDomainDTO {

        public Guid? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
