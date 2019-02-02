using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models.Tasks {
    public class FinalReviewStudentTaskRequest {

        public string Review { get; set; }

        [Required]
        public int Grade { get; set; }
    }
}
