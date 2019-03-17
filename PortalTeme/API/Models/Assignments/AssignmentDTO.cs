using PortalTeme.API.Models.Courses;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.API.Models.Assignments {

    public class AssignmentBaseDTO {

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        public AssignmentType Type { get; set; }

        public int? NumberOfDuplicates { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class AssignmentDTO : AssignmentBaseDTO {

        public Guid Id { get; set; }

        public CourseEditDTO Course { get; set; }

        public List<AssignmentTaskDTO> Tasks { get; set; }
    }

    public class AssignmentEditDTO : AssignmentBaseDTO {
        public Guid? Id { get; set; }

        public CourseRefDTO Course { get; set; }
    }

    public class StudentAssignmentDTO : AssignmentDTO {
        public AssignmentTaskDTO AssignedTask { get; set; }
    }

    public class AssignmentTaskBaseDTO {

        public Guid AssignmentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }

    public class AssignmentTaskUpdateRequest : AssignmentTaskCreateRequest {
        public Guid Id { get; set; }
    }

    public class AssignmentTaskCreateRequest : AssignmentTaskBaseDTO {
        public string AssignedTo { get; set; }
    }

    public class AssignmentTaskDTO : AssignmentTaskBaseDTO {
        public Guid Id { get; set; }
        public List<UserDTO> StudentsAssigned { get; set; }
    }

    public class StudentAssignedTaskDTO {

        public Guid Id { get; set; }

        public AssignmentTaskDTO Task { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string StudentId { get; set; }
        public UserDTO Student { get; set; }

        public StudentAssignedTaskState State { get; set; }

        public string Review { get; set; }

        public int? FinalGrading { get; set; }

        public List<TaskSubmissionDTO> Submissions { get; set; }
    }

    public class TaskSubmissionDTO {

        public Guid? Id { get; set; }

        public Guid StudentTaskId { get; set; }

        public DateTime DateAdded { get; set; }

        public TaskSubmissionState State { get; set; }

        public string Review { get; set; }

        public int? Grading { get; set; }

        public string Description { get; set; }

        public List<TaskSubmissionFileDTO> Files { get; set; }
    }

    public class TaskSubmissionFileDTO {

        public Guid? Id { get; set; }

        public Guid FileId { get; set; }

        public TaskSubmissionFileType FileType { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
    }
}
