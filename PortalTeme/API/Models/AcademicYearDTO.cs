using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Models {
    public class AcademicYearDTO {

        public Guid? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

    }
}
