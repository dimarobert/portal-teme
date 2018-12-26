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
    [Authorize(Policy = AuthorizationConstants.AdministratorPolicy)]
    public class CourseDefinitionsController : ControllerBase {
        private readonly PortalTemeContext _context;

        public CourseDefinitionsController(PortalTemeContext context) {
            _context = context;
        }

        // GET: api/CourseDefinitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDefinition>>> GetCourseDefinitions() {
            return await _context.CourseDefinitions.ToListAsync();
        }

        // GET: api/CourseDefinitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDefinition>> GetCourseDefinition(Guid id) {
            var courseDefinition = await _context.CourseDefinitions.FindAsync(id);

            if (courseDefinition is null)
                return NotFound();

            return courseDefinition;
        }

        // PUT: api/CourseDefinitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseDefinition(Guid id, CourseDefinition courseDefinition) {
            if (id != courseDefinition.Id)
                return BadRequest();

            _context.Entry(courseDefinition).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!CourseDefinitionExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/CourseDefinitions
        [HttpPost]
        public async Task<ActionResult<CourseDefinition>> PostCourseDefinition(CourseDefinition courseDefinition) {
            _context.CourseDefinitions.Add(courseDefinition);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourseDefinition", new { id = courseDefinition.Id }, courseDefinition);
        }

        // DELETE: api/CourseDefinitions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseDefinition>> DeleteCourseDefinition(Guid id) {
            var courseDefinition = await _context.CourseDefinitions.FindAsync(id);
            if (courseDefinition is null)
                return NotFound();

            _context.CourseDefinitions.Remove(courseDefinition);
            await _context.SaveChangesAsync();

            return courseDefinition;
        }

        private bool CourseDefinitionExists(Guid id) {
            return _context.CourseDefinitions.Any(e => e.Id == id);
        }
    }
}
