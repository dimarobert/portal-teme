using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Models {
    public class GroupDTO {

        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }


        [Required]
        public Guid Domain { get; set; }

        [Required]
        public Guid Year { get; set; }
    }
}
