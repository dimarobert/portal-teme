using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalTeme.Data.Models {
    public class CourseGroup {

        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public Guid GroupId { get; set; }
        public Group Group { get; set; }

    }
}
