using PortalTeme.API.Models;
using PortalTeme.API.Models.Assignments;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
using PortalTeme.Services;
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

        Task<StudentAssignedTaskDTO> MapStudentAssignedTask(StudentTaskProjection studentTask);
    }

    public class TaskMapper : ITaskMapper {
        private readonly ICourseMapper courseMapper;
        private readonly IFileService fileService;

        public TaskMapper(ICourseMapper courseMapper, IFileService fileService) {
            this.courseMapper = courseMapper;
            this.fileService = fileService;
        }

        public async Task<StudentAssignedTaskDTO> MapStudentAssignedTask(StudentTaskProjection studentTask) {
            var submissions = new List<TaskSubmissionDTO>();
            foreach (var sub in studentTask.Submissions)
                submissions.Add(await MapSubmission(studentTask.Id, sub));

            return new StudentAssignedTaskDTO {
                Id = studentTask.Id,
                StudentId = studentTask.StudentId,
                Student = courseMapper.MapStudent(studentTask.Student),
                Task = MapTask(studentTask.Task),
                Grading = studentTask.Grading,
                State = studentTask.State,
                Submissions = submissions
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

        private async Task<TaskSubmissionDTO> MapSubmission(Guid studentTaskId, TaskSubmission submission) {
            var files = new List<TaskSubmissionFileDTO>();
            foreach (var file in submission.Files)
                files.Add(await MapSubmissionFile(file));

            return new TaskSubmissionDTO {
                Id = submission.Id,
                StudentTaskId = studentTaskId,
                DateAdded = submission.DateAdded,
                Files = files
            };
        }

        private async Task<TaskSubmissionFileDTO> MapSubmissionFile(TaskSubmissionFile file) {
            var fileSize = await fileService.GetFileSize(file.File);
            return new TaskSubmissionFileDTO {
                Id = file.Id,
                Name = file.File.FileName,
                Extension = file.File.Extension,
                Size = fileSize,
                Description = file.Description,
                FileType = file.FileType
            };
        }

    }
}
