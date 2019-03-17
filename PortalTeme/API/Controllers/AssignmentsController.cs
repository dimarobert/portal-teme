using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Assignments;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using PortalTeme.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentsController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly UserManager<User> userManager;
        private readonly IAuthorizationService authorizationService;
        private readonly IAssignmentMapper assignmentMapper;
        private readonly ITaskMapper taskMapper;
        private readonly IUrlSlugService slugService;

        public AssignmentsController(PortalTemeContext context, UserManager<User> userManager, IAuthorizationService authorizationService, IAssignmentMapper assignmentMapper, ITaskMapper taskMapper, IUrlSlugService slugService) {
            _context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
            this.assignmentMapper = assignmentMapper;
            this.taskMapper = taskMapper;
            this.slugService = slugService;
        }

        // GET: api/Assignments
        [HttpGet("ForCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDTO>>> GetAssignmentsForCourse(Guid courseId) {
            var courseAssignments = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
                .Include(a => a.Course).ThenInclude(c => c.Professor)
                .Include(a => a.AssignmentTasks).ThenInclude(t => t.StudentsAssigned).ThenInclude(sa => sa.Student).ThenInclude(si => si.User)
                .Where(assignment => assignment.Course.Id == courseId)
                .ToListAsync();

            if (!courseAssignments.Any())
                return Enumerable.Empty<AssignmentDTO>().ToList();

            var authorization = await authorizationService.AuthorizeAsync(User, courseAssignments.First().Course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            var results = courseAssignments
                .Select(assignment => assignmentMapper.MapAssignment(assignment))
                .ToList();

            return results;
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentDTO>> GetAssignment(Guid id) {
            var assignment = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
                .Include(a => a.Course).ThenInclude(c => c.Professor)
                .Include(a => a.AssignmentTasks).ThenInclude(t => t.StudentsAssigned).ThenInclude(sa => sa.Student).ThenInclude(si => si.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, assignment.Course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            return assignmentMapper.MapAssignment(assignment);
        }

        // GET: api/Assignments/course-slug-1/slug/assgnment-1
        [HttpGet("{courseSlug}/slug/{slug}")]
        public async Task<ActionResult<StudentAssignmentDTO>> GetAssignmentBySlug(string courseSlug, string slug) {
            if (string.IsNullOrWhiteSpace(slug)) {
                ModelState.AddModelError(string.Empty, "The 'slug' path parameter cannot be null, empty or whitespace.");
                return BadRequest(ModelState);
            }

            var assignment = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
                .Include(a => a.Course).ThenInclude(c => c.Professor)
                .Include(a => a.AssignmentTasks).ThenInclude(t => t.StudentsAssigned).ThenInclude(sa => sa.Student).ThenInclude(si => si.User)
                .FirstOrDefaultAsync(a => a.Course.CourseInfo.Slug == courseSlug && a.Slug == slug);

            if (assignment is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, assignment.Course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            var user = await userManager.GetUserAsync(User);

            if (assignment.StartDate > DateTime.UtcNow)
                assignment.AssignmentTasks = new List<AssignmentTask>();

            return assignmentMapper.MapStudentAssignment(assignment, user);
        }

        // PUT: api/Assignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignment(Guid id, AssignmentEditDTO assignment) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != assignment.Id)
                return BadRequest();

            var dbAssignment = assignmentMapper.MapAssignmentEditDTO(assignment);

            dbAssignment.Slug = slugService.TransformText(dbAssignment.Name);

            var collidingAssignment = await _context.Assignments
                .Where(a => a.Course.Id == dbAssignment.Course.Id && a.Id != dbAssignment.Id && a.Slug == dbAssignment.Slug)
                .FirstOrDefaultAsync();

            if (collidingAssignment != null) {
                ModelState.AddModelError("name", $"Failed to generate unique url slug from the assignment name. The generated slug ({dbAssignment.Slug}) is used by another assignment of this course.");
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Assistants)
                .Include(c => c.Groups)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == dbAssignment.Course.Id);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            var existingAssignment = await _context.Assignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == dbAssignment.Id);

            dbAssignment.DateAdded = existingAssignment.DateAdded;
            dbAssignment.LastUpdated = DateTime.UtcNow;

            _context.Entry(dbAssignment).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!AssignmentExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Assignments
        [HttpPost]
        public async Task<ActionResult<AssignmentDTO>> PostAssignment(AssignmentEditDTO assignment) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dbAssignment = assignmentMapper.MapAssignmentEditDTO(assignment);

            dbAssignment.Slug = slugService.TransformText(dbAssignment.Name);

            var existingAssignment = await _context.Assignments
                .Where(a => a.Course.Id == dbAssignment.Course.Id && a.Slug == dbAssignment.Slug)
                .FirstOrDefaultAsync();

            if (existingAssignment != null) {
                ModelState.AddModelError("name", $"Failed to generate unique url slug from the assignment name. The generated slug ({dbAssignment.Slug}) is used by another assignment of this course.");
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Assistants)
                .Include(c => c.Groups)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == dbAssignment.Course.Id);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanCreateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            dbAssignment.DateAdded = DateTime.UtcNow;
            dbAssignment.LastUpdated = DateTime.UtcNow;

            _context.Assignments.Add(dbAssignment);
            await _context.SaveChangesAsync();

            dbAssignment = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
                .Include(a => a.Course).ThenInclude(c => c.Professor)
                .Include(a => a.AssignmentTasks).ThenInclude(t => t.StudentsAssigned).ThenInclude(sa => sa.Student).ThenInclude(si => si.User)
                .FirstOrDefaultAsync(a => a.Id == dbAssignment.Id);

            return CreatedAtAction("GetAssignment", new { id = dbAssignment.Id }, assignmentMapper.MapAssignment(dbAssignment));
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(Guid id) {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .ThenInclude(c => c.CourseInfo)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment is null)
                return NotFound();

            var course = await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Assistants)
                .Include(c => c.Groups)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == assignment.Course.Id);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanDeleteCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("{assignmentId}/task")]
        public async Task<ActionResult<AssignmentTaskDTO>> PostTask(AssignmentTaskCreateRequest taskRequest) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assignmentTask = taskMapper.MapTaskCreateRequest(taskRequest);

            var assginment = await _context.Assignments.FindAsync(assignmentTask.AssignmentId);

            if (ShouldCustomAssignStudent(taskRequest, assginment)) {
                var studentUser = await _context.Students.FirstOrDefaultAsync(s => s.UserId == taskRequest.AssignedTo);

                if (studentUser is null) {
                    ModelState.AddModelError("assignedTo", "The selected student to be assigned could not be found.");
                    return BadRequest(ModelState);
                }

                assignmentTask.StudentsAssigned.Add(new StudentAssignedTask {
                    State = StudentAssignedTaskState.Assigned,
                    Task = assignmentTask,
                    StudentId = taskRequest.AssignedTo
                });
            }

            _context.AssignmentTasks.Add(assignmentTask);
            await _context.SaveChangesAsync();

            return taskMapper.MapTask(assignmentTask);
        }



        [HttpPut("{assignmentId}/task/{id}")]
        public async Task<IActionResult> PutTask(Guid id, AssignmentTaskUpdateRequest taskRequest) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != taskRequest.Id)
                return BadRequest();

            var assignmentTask = taskMapper.MapTaskUpdateRequest(taskRequest);

            _context.Entry(assignmentTask).State = EntityState.Modified;

            var assignment = await _context.Assignments.FindAsync(assignmentTask.AssignmentId);

            if (assignment.Type == AssignmentType.CustomAssignedTasks) {
                var prevAssignedStudents = await _context.StudentAssignedTasks.Where(sat => sat.TaskId == assignmentTask.Id).ToListAsync();
                if (prevAssignedStudents.Any())
                    _context.StudentAssignedTasks.RemoveRange(prevAssignedStudents.Where(s => s.StudentId != taskRequest.AssignedTo));

                if (!string.IsNullOrWhiteSpace(taskRequest.AssignedTo) && !prevAssignedStudents.Any(s => s.StudentId == taskRequest.AssignedTo)) {
                    var studentUser = await _context.Students.FirstOrDefaultAsync(s => s.UserId == taskRequest.AssignedTo);

                    if (studentUser is null) {
                        ModelState.AddModelError("assignedTo", "The selected student to be assigned could not be found.");
                        return BadRequest(ModelState);
                    }

                    assignmentTask.StudentsAssigned.Add(new StudentAssignedTask {
                        State = StudentAssignedTaskState.Assigned,
                        Task = assignmentTask,
                        StudentId = taskRequest.AssignedTo
                    });
                }
            }

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!TaskExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{assignmentId}/task/{id}")]
        public async Task<ActionResult<AssignmentTaskDTO>> DeleteTask(Guid id) {
            var task = await _context.AssignmentTasks
                 .FirstOrDefaultAsync(t => t.Id == id);

            _context.AssignmentTasks.Remove(task);
            await _context.SaveChangesAsync();

            return taskMapper.MapTask(task);
        }


        private bool AssignmentExists(Guid id) {
            return _context.Assignments.Any(e => e.Id == id);
        }

        private bool TaskExists(Guid id) {
            return _context.AssignmentTasks.Any(e => e.Id == id);
        }

        private bool ShouldCustomAssignStudent(AssignmentTaskCreateRequest taskRequest, Assignment assginment) {
            return assginment.Type == AssignmentType.CustomAssignedTasks && !string.IsNullOrWhiteSpace(taskRequest.AssignedTo);
        }

    }
}
