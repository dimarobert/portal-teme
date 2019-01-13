using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Assignments;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
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
        private readonly IAuthorizationService authorizationService;
        private readonly IAssignmentMapper assignmentMapper;
        private readonly IUrlSlugService slugService;

        public AssignmentsController(PortalTemeContext context, IAuthorizationService authorizationService, IAssignmentMapper assignmentMapper, IUrlSlugService slugService) {
            _context = context;
            this.authorizationService = authorizationService;
            this.assignmentMapper = assignmentMapper;
            this.slugService = slugService;
        }

        // GET: api/Assignments
        [HttpGet("ForCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDTO>>> GetAssignmentsForCourse(Guid courseId) {
            var courseAssignments = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
                .Include(a => a.Course).ThenInclude(c => c.Professor)
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
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, assignment.Course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            return assignmentMapper.MapAssignment(assignment);
        }

        // GET: api/Assignments/slug/assgnment-1
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<AssignmentDTO>> GetAssignmentBySlug(string slug) {
            if (string.IsNullOrWhiteSpace(slug)) {
                ModelState.AddModelError(string.Empty, "The 'slug' path parameter cannot be null, empty or whitespace.");
                return BadRequest(ModelState);
            }

            var assignment = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
                .Include(a => a.Course).ThenInclude(c => c.Professor)
                .FirstOrDefaultAsync(a => a.Slug == slug);

            if (assignment is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, assignment.Course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            return assignmentMapper.MapAssignment(assignment);
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

            return CreatedAtAction("GetAssignment", new { id = dbAssignment.Id }, assignmentMapper.MapAssignment(dbAssignment));
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AssignmentDTO>> DeleteAssignment(Guid id) {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .ThenInclude(c => c.CourseInfo)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment is null)
                return NotFound();

            var course = _context.Courses
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

            return assignmentMapper.MapAssignment(assignment);
        }

        private bool AssignmentExists(Guid id) {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}
