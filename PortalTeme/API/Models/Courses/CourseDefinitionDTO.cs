using PortalTeme.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Models.Courses {
    public class CourseDefinitionDTO {

        public Guid? Id { get; set; }

        [Required]
        public Guid Year { get; set; }

        [Required]
        public Semester Semester { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings =false)]
        public string Acronym { get; set; }
    }
}
