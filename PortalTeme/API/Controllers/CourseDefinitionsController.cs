using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Courses;
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
    [Authorize(Policy = AuthorizationConstants.AdministratorPolicy)]
    public class CourseDefinitionsController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly ICacheService cache;
        private readonly ICourseMapper courseMapper;
        private readonly IUrlSlugService slugService;

        public CourseDefinitionsController(PortalTemeContext context, ICacheService cache, ICourseMapper courseMapper, IUrlSlugService slugService) {
            _context = context;
            this.cache = cache;
            this.courseMapper = courseMapper;
            this.slugService = slugService;
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
            courseDef.Slug = slugService.TransformText(courseDef.Name);

            var existingDef = await _context.CourseDefinitions.FirstOrDefaultAsync(c => c.Id != courseDef.Id && c.Slug == courseDef.Slug);
            if (!(existingDef is null)) {
                ModelState.AddModelError("name", $"Failed to generate unique url slug from the course name. The generated slug ({courseDef.Slug}) is used by another course.");
                return BadRequest(ModelState);
            }

            _context.Entry(courseDef).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();

                await cache.ClearCourseDefinitionCacheAsync(id);
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
            courseDef.Slug = slugService.TransformText(courseDef.Name);

            var existingDef = await _context.CourseDefinitions.FirstOrDefaultAsync(c => c.Slug == courseDef.Slug);
            if (!(existingDef is null)) {
                ModelState.AddModelError("name", $"Failed to generate unique url slug. The generated slug ({courseDef.Slug}) is used by another course.");
                return BadRequest(ModelState);
            }

            _context.CourseDefinitions.Add(courseDef);
            await _context.SaveChangesAsync();

            await cache.ClearCoursesRefCacheAsync();

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

            await cache.ClearCourseDefinitionCacheAsync(id);

            return courseMapper.MapDefinition(courseDefinition);
        }

        private bool CourseDefinitionExists(Guid id) {
            return _context.CourseDefinitions.Any(e => e.Id == id);
        }
    }
}
