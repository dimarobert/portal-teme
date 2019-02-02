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
                    Review = sat.Review,
                    FinalGrading = sat.FinalGrading,
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
                    FinalGrading = sat.FinalGrading,
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

        // POST: api/Submissions/5/FinalGrade
        [HttpPost("{studentTaskId}/FinalGrade")]
        public async Task<IActionResult> PostFinalGrade(Guid studentTaskId, FinalReviewStudentTaskRequest request) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentTask = await _context.StudentAssignedTasks
               .FirstOrDefaultAsync(ts => ts.Id == studentTaskId);

            if (studentTask is null)
                return NotFound();

            //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
            //    if (!authorization.Succeeded)
            //        return Forbid();

            studentTask.Review = request.Review;
            studentTask.FinalGrading = request.Grade;
            studentTask.State = StudentAssignedTaskState.FinalGraded;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
