using PortalTeme.API.Models;
using PortalTeme.API.Models.Assignments;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.API.Mappers {

    public interface ITaskMapper {

        AssignmentTaskDTO MapTask(AssignmentTask task);
        AssignmentTask MapTaskEditDTO(AssignmentTaskEditDTO task);
        AssignmentTask MapTaskDTO(AssignmentTaskDTO task);

        StudentAssignedTaskDTO MapStudentAssignedTask(StudentTaskProjection studentTask);
    }

    public class TaskMapper : ITaskMapper {
        private readonly ICourseMapper courseMapper;

        public TaskMapper(ICourseMapper courseMapper) {
            this.courseMapper = courseMapper;
        }

        public StudentAssignedTaskDTO MapStudentAssignedTask(StudentTaskProjection studentTask) {
            return new StudentAssignedTaskDTO {
                Id = studentTask.Id,
                StudentId = studentTask.StudentId,
                Student = courseMapper.MapStudent(studentTask.Student),
                Task = MapTask(studentTask.Task),
                Grading = studentTask.Grading,
                State = studentTask.State,
                Submissions = studentTask.Submissions.Select(sub => MapSubmission(studentTask.Id, sub)).ToList()
            };
        }

        public AssignmentTaskDTO MapTask(AssignmentTask task) {
            return new AssignmentTaskDTO {
                Id = task.Id,
                AssignmentId = task.AssignmentId,
                Name = task.Name,
                Description = task.Description,
                StudentsAssigned = task.StudentsAssigned?.Select(sa => courseMapper.MapStudent(sa.Student)).ToList() ?? new List<UserDTO>()
            };
        }

        public AssignmentTask MapTaskEditDTO(AssignmentTaskEditDTO task) {
            return new AssignmentTask {
                Id = task.Id ?? Guid.Empty,
                AssignmentId = task.AssignmentId,
                Name = task.Name,
                Description = task.Description
            };
        }

        public AssignmentTask MapTaskDTO(AssignmentTaskDTO task) {
            return new AssignmentTask {
                Id = task.Id,
                AssignmentId = task.AssignmentId,
                Name = task.Name,
                Description = task.Description
            };
        }

        private TaskSubmissionDTO MapSubmission(Guid studentTaskId, TaskSubmission submission) {
            return new TaskSubmissionDTO {
                Id = submission.Id,
                StudentTaskId = studentTaskId,
                DateAdded = submission.DateAdded,
                Files = submission.Files.Select(file => MapSubmissionFile(file)).ToList()
            };
        }

        private TaskSubmissionFileDTO MapSubmissionFile(TaskSubmissionFile file) {
            return new TaskSubmissionFileDTO {
                Id = file.Id,
                Description = file.Description,
                FileType = file.FileType
            };
        }

    }
}
