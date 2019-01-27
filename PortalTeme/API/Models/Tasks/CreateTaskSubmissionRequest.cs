using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.API.Models.Tasks
{
    public class CreateTaskSubmissionRequest
    {
        [Required]
        public Guid StudentTaskId { get; set; }

        public List<UploadedTempFile> UploadedFiles { get; set; }
    }
}
