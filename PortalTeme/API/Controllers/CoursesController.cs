using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly IAuthorizationService authorizationService;

        public CoursesController(PortalTemeContext context, IAuthorizationService authorizationService) {
            _context = context;
            this.authorizationService = authorizationService;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses() {

            var courses = await _context.Courses.ToListAsync();
            var results = new List<Course>();
            foreach (var course in courses) {
                if ((await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanViewCoursesPolicy)).Succeeded)
                    results.Add(course);
            }

            return results;
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(Guid id) {
            var course = await _context.Courses.FindAsync(id);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanViewCoursesPolicy);
            if (!authorization.Succeeded)
                return Forbid();

            if (course is null)
                return NotFound();

            return course;
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(Guid id, Course course) {
            if (id != course.Id)
                return BadRequest();

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Entry(course).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!CourseExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course) {

            var authorization = await authorizationService.AuthorizeAsync(User, AuthorizationConstants.CanCreateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Course>> DeleteCourse(Guid id) {
            var course = await _context.Courses.FindAsync(id);
            if (course is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanDeleteCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }

        private bool CourseExists(Guid id) {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
