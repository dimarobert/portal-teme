using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Assignments;
using PortalTeme.API.Models.Tasks;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
using PortalTeme.Helpers;
using PortalTeme.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthorizationConstants.AdministratorPolicy)]
    public class StudentAssignedTasksController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly UserManager<User> userManager;
        private readonly ITaskMapper taskMapper;
        private readonly IFileService fileService;
        private readonly IAuthorizationService authorizationService;

        public StudentAssignedTasksController(PortalTemeContext context, UserManager<User> userManager,
            ITaskMapper taskMapper,
            IFileService fileService,
            IAuthorizationService authorizationService) {
            _context = context;
            this.userManager = userManager;
            this.taskMapper = taskMapper;
            this.fileService = fileService;
            this.authorizationService = authorizationService;
        }

        //// GET: api/AssignmentEntries/ForAssignment/5
        //[HttpGet("ForAssignment/{assignmentId}")]
        //public async Task<ActionResult<IEnumerable<AssignmentEntryDTO>>> GetAssignmentEntries(Guid assignmentId) {
        //    var entries = await _context.AssignmentEntries
        //        .Where(ae => ae.AssignedTask.AssignmentId == assignmentId)
        //        .Select(ae => new AssignmentEntryProjection {
        //            Id = ae.Id,
        //            AssignmentTaskId = ae.AssignmentTask.Id,
        //            CourseId = ae.AssignmentTask.Assignment.Course.Id,
        //            StudentId = ae.Student.UserId,
        //            State = ae.State,
        //            Grading = ae.Grading,
        //            Versions = ae.Versions
        //        })
        //        .ToListAsync();

        //    var results = new List<AssignmentEntryDTO>();
        //    foreach (var entry in entries) {
        //        var authorization = await authorizationService.AuthorizeAsync(User, entry, AuthorizationConstants.CanViewAssignmentEntriesPolicy);
        //        if (!authorization.Succeeded)
        //            results.Add(assignmentMapper.MapAssignmentEntryProjection(entry));
        //    }

        //    return results;
        //}

        // GET: api/StudentAssignedTasks/5
        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<StudentAssignedTaskDTO>> GetStudentAssignedTask(Guid assignmentId) {

            if (!User.Identity.IsAuthenticated)
                return Challenge();

            var studentId = userManager.GetUserId(User);

            var studentTask = await _context.StudentAssignedTasks
                .Where(sat => sat.StudentId == studentId && sat.Task.AssignmentId == assignmentId)
                .Select(sat => new StudentTaskProjection {
                    Id = sat.Id,
                    Task = sat.Task,
                    StudentId = sat.StudentId,
                    Student = sat.Student,
                    CourseId = sat.Task.Assignment.Course.Id,
                    State = sat.State,
                    Grading = sat.Grading,
                    Submissions = sat.Submissions
                })
                .FirstOrDefaultAsync();

            if (studentTask is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, studentTask, AuthorizationConstants.CanViewStudentTaskPolicy);
            if (!authorization.Succeeded)
                return Forbid();

            await _context.Entry(studentTask.Student)
                .Reference(t => t.User)
                .LoadAsync();

            await _context.TaskSubmissions
                .Where(t => studentTask.Submissions.Contains(t))
                .Include(s => s.Files).ThenInclude(f => f.File)
                .ToListAsync();

            return await taskMapper.MapStudentAssignedTask(studentTask);
        }

        //// PUT: api/AssignmentEntries/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAssignmentEntry(Guid id, AssignmentEntryDTO assignmentEntryDto) {
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (id != assignmentEntryDto.Id)
        //        return BadRequest();

        //    var assignmentEntry = assignmentMapper.MapAssignmentEntryProjectionDTO(assignmentEntryDto);

        //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
        //    if (!authorization.Succeeded)
        //        return Forbid();

        //    _context.Entry(assignmentEntry).State = EntityState.Modified;

        //    try {
        //        await _context.SaveChangesAsync();
        //    } catch (DbUpdateConcurrencyException) {
        //        if (!AssignmentEntryExists(id))
        //            return NotFound();
        //        else
        //            throw;
        //    }

        //    return NoContent();
        //}

        // POST: api/StudentAssignedTasks/5/Assign
        [HttpPost("{taskId}/Assign")]
        public async Task<ActionResult<StudentAssignedTaskDTO>> PostStudentAssignedTask(Guid taskId) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _context.AssignmentTasks
                .Include(t => t.Assignment)
                .Include(t => t.StudentsAssigned)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task is null)
                return NotFound();

            //var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
            //if (!authorization.Succeeded)
            //    return Forbid();

            if (task.Assignment.StartDate > DateTime.UtcNow) {
                ModelState.AddModelError(string.Empty, "The assignment has not started yet.");
                return BadRequest(ModelState);
            }

            if (task.Assignment.EndDate < DateTime.UtcNow) {
                ModelState.AddModelError(string.Empty, "The assignment deadline has passed.");
                return BadRequest(ModelState);
            }

            if (task.Assignment.Type == AssignmentType.CustomAssignedTasks) {
                ModelState.AddModelError(string.Empty, "Tasks cannot be self assigned for this assignment.");
                return BadRequest(ModelState);
            }

            if (task.Assignment.Type == AssignmentType.SingleChoiceList && task.StudentsAssigned.Any()) {
                ModelState.AddModelError(string.Empty, "A task can only be assigned to a single student.");
                return BadRequest(ModelState);
            }

            if (task.Assignment.Type == AssignmentType.MultipleChoiceList && task.Assignment.NumberOfDuplicates <= task.StudentsAssigned.Count) {
                ModelState.AddModelError(string.Empty, "This task has reached maximum student assignments.");
                return BadRequest(ModelState);
            }

            var studentId = userManager.GetUserId(User);

            var studentAssignedTask = new StudentAssignedTask {
                State = StudentAssignedTaskState.Assigned,
                StudentId = studentId,
                TaskId = task.Id
            };

            _context.StudentAssignedTasks.Add(studentAssignedTask);
            await _context.SaveChangesAsync();

            var studentTask = await _context.StudentAssignedTasks
                .Where(sat => sat.Id == studentAssignedTask.Id)
                .Select(sat => new StudentTaskProjection {
                    Task = sat.Task,
                    Student = sat.Student,
                    StudentId = sat.StudentId,
                    CourseId = sat.Task.Assignment.Course.Id,
                    State = sat.State,
                    Grading = sat.Grading,
                    Submissions = sat.Submissions
                })
                .FirstOrDefaultAsync();

            await _context.Entry(studentTask.Task)
                .Collection(t => t.StudentsAssigned)
                .Query()
                .Include(sa => sa.Student).ThenInclude(s => s.User)
                .LoadAsync();

            return CreatedAtAction("GetAssignmentEntry", new { id = studentAssignedTask.Id }, await taskMapper.MapStudentAssignedTask(studentTask));
        }

        // POST: api/StudentAssignedTasks/5/Assign
        [HttpPost("{taskId}/Submit")]
        public async Task<ActionResult<StudentAssignedTaskDTO>> PostSubmitTask(Guid taskId, CreateTaskSubmissionRequest request) {

            if (taskId != request.StudentTaskId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentTask = await _context.StudentAssignedTasks
                .FirstOrDefaultAsync(st => st.Id == request.StudentTaskId);

            if (studentTask is null)
                return NotFound();

            if (studentTask.StudentId != userManager.GetUserId(User))
                return Forbid();

            var submission = new TaskSubmission {
                AssignedTask = studentTask,
                DateAdded = DateTime.Now,
                Files = new List<TaskSubmissionFile>(),
                Description = request.Description
            };

            _context.TaskSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            foreach (var uploadedFile in request.UploadedFiles) {

                var submissionFolder = $"StudentSubmissions/{studentTask.StudentId}/{studentTask.Id}/{submission.Id}";

                // TODO: validate file extension.
                var (fileName, extension) = FilesHelpers.GetFileNameAndExtension(uploadedFile.OriginalName);

                var file = await fileService.MoveTempFile(uploadedFile.TempFileName, submissionFolder, uploadedFile.OriginalName);

                submission.Files.Add(new TaskSubmissionFile {
                    FileId = file.Id,
                    FileType = TaskSubmissionFileType.SourceCode,
                    TaskSubmission = submission
                });
            }

            await _context.SaveChangesAsync();

            return null;
        }

        [HttpPost("{taskId}/Grade")]
        public async Task<IActionResult> PostGradeTask(Guid taskId, GradeTaskSubmissionRequest request) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentTask = await _context.StudentAssignedTasks
               .FirstOrDefaultAsync(st => st.Id == taskId);

            if (studentTask is null)
                return NotFound();

            //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
            //    if (!authorization.Succeeded)
            //        return Forbid();

            studentTask.Grading = request.Grade;
            studentTask.State = StudentAssignedTaskState.Graded;
            await _context.SaveChangesAsync();

            return Ok();
        }
        //// DELETE: api/AssignmentEntries/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<AssignmentEntryDTO>> DeleteAssignmentEntry(Guid id) {
        //    var assignmentEntry = await _context.AssignmentEntries.FindAsync(id);
        //    if (assignmentEntry is null)
        //        return NotFound();

        //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
        //    if (!authorization.Succeeded)
        //        return Forbid();

        //    var projection = await _context.AssignmentEntries
        //          .Where(ae => ae.Id == assignmentEntry.Id)
        //          .Select(ae => new AssignmentEntryProjection {
        //              Id = ae.Id,
        //              AssignmentTaskId = ae.AssignedTask.Id,
        //              CourseId = ae.AssignedTask.Assignment.Course.Id,
        //              StudentId = ae.Student.UserId,
        //              State = ae.State,
        //              Grading = ae.Grading,
        //              Versions = ae.Versions
        //          })
        //          .FirstOrDefaultAsync();

        //    _context.AssignmentEntries.Remove(assignmentEntry);
        //    await _context.SaveChangesAsync();

        //    return assignmentMapper.MapAssignmentEntryProjection(projection);
        //}

        //private bool AssignmentEntryExists(Guid id) {
        //    return _context.AssignmentEntries.Any(e => e.Id == id);
        //}
    }
}
