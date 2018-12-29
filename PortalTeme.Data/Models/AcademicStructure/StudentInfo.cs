using PortalTeme.Data.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortalTeme.Data.Models {
    public class StudentInfo {

        [Key]
        public string UserId { get; set; }

        [Required]
        public Group Group { get; set; }

        [Required]
        public Semester Semester { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }
    }

}
