using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Assignments;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
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

        public AssignmentsController(PortalTemeContext context, IAuthorizationService authorizationService, IAssignmentMapper assignmentMapper) {
            _context = context;
            this.authorizationService = authorizationService;
            this.assignmentMapper = assignmentMapper;
        }

        // GET: api/Assignments
        [HttpGet("{courseId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDTO>>> GetAssignments(Guid courseId) {
            var courseAssignments = await _context.Assignments
                .Include(a => a.Course).ThenInclude(c => c.CourseInfo)
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
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, assignment.Course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            return assignmentMapper.MapAssignment(assignment);
        }

        // PUT: api/Assignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignment(Guid id, AssignmentDTO assignment) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != assignment.Id)
                return BadRequest();

            var dbAssignment = assignmentMapper.MapAssignmentDTO(assignment);

            var course = _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Assistants)
                .Include(c => c.Groups)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == dbAssignment.Course.Id);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

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
        public async Task<ActionResult<AssignmentDTO>> PostAssignment(AssignmentDTO assignment) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dbAssignment = assignmentMapper.MapAssignmentDTO(assignment);

            var course = _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Assistants)
                .Include(c => c.Groups)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == dbAssignment.Course.Id);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanCreateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

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
