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

        public StudentAssignedTaskDTO MapStudentAssignedTask(StudentTaskProjection studentTask) {
            return new StudentAssignedTaskDTO {
                StudentId = studentTask.StudentId,
                Task = MapTask(studentTask.Task),
                Grading = studentTask.Grading,
                State = studentTask.State,
                Submissions = studentTask.Submissions.Select(sub => MapSubmission(sub)).ToList()
            };
        }

        public AssignmentTaskDTO MapTask(AssignmentTask task) {
            return new AssignmentTaskDTO {
                Id = task.Id,
                AssignmentId = task.AssignmentId,
                Name = task.Name,
                Description = task.Description
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

        private TaskSubmissionDTO MapSubmission(TaskSubmission submission) {
            return new TaskSubmissionDTO {
                Id = submission.Id,
                StudentTaskId = submission.AssignedTask.Id,
                DateAdded = submission.DateAdded,
                Files = submission.Files.Select(file => MapSubmissionFile(file)).ToList()
            };
        }

        private TaskSubmissionFileDTO MapSubmissionFile(TaskSubmissionFile file) {
            return new TaskSubmissionFileDTO {
                Id = file.Id,
                Name = file.Name,
                Description = file.Description,
                FileType = file.FileType
            };
        }

    }
}
