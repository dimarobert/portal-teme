using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
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
        private readonly ICourseMapper courseMapper;

        public CourseDefinitionsController(PortalTemeContext context, ICourseMapper courseMapper) {
            _context = context;
            this.courseMapper = courseMapper;
        }

        // GET: api/CourseDefinitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDefinitionDTO>>> GetCourseDefinitions() {
            return (await _context.CourseDefinitions
                .Include(c => c.Year)
                .ToListAsync())
                .Select(cDef => courseMapper.MapDefinition(cDef))
                .ToList();
        }

        // GET: api/CourseDefinitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDefinitionDTO>> GetCourseDefinition(Guid id) {
            var courseDefinition = await _context.CourseDefinitions.FindAsync(id);

            if (courseDefinition is null)
                return NotFound();

            return courseMapper.MapDefinition(courseDefinition);
        }

        // PUT: api/CourseDefinitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseDefinition(Guid id, CourseDefinitionDTO courseDefinition) {
            if (id != courseDefinition.Id)
                return BadRequest();

            var year = await _context.AcademicYears.FirstAsync(y => y.Id == courseDefinition.Year);
            if (year is null) {
                ModelState.AddModelError("year", "Invalid course year provided.");
                return BadRequest(ModelState);
            }

            var courseDef = courseMapper.MapDefinitionDTO(courseDefinition, year);

            _context.Entry(courseDef).State = EntityState.Modified;

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
        public async Task<ActionResult<CourseDefinitionDTO>> PostCourseDefinition(CourseDefinitionDTO courseDefinition) {
            var year = await _context.AcademicYears.FirstAsync(y => y.Id == courseDefinition.Year);
            if (year is null) {
                ModelState.AddModelError("year", "Invalid course year provided.");
                return BadRequest(ModelState);
            }
            var courseDef = courseMapper.MapDefinitionDTO(courseDefinition, year);

            _context.CourseDefinitions.Add(courseDef);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourseDefinition",
                new { id = courseDef.Id },
                courseMapper.MapDefinition(courseDef)
            );
        }

        // DELETE: api/CourseDefinitions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseDefinitionDTO>> DeleteCourseDefinition(Guid id) {
            var courseDefinition = await _context.CourseDefinitions
                .Include(c => c.Year)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (courseDefinition is null)
                return NotFound();

            _context.CourseDefinitions.Remove(courseDefinition);
            await _context.SaveChangesAsync();

            return courseMapper.MapDefinition(courseDefinition);
        }

        private bool CourseDefinitionExists(Guid id) {
            return _context.CourseDefinitions.Any(e => e.Id == id);
        }
    }
}
