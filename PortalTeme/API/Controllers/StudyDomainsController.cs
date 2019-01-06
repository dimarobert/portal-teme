using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Courses;
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
    public class StudyDomainsController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly ICourseMapper courseMapper;

        public StudyDomainsController(PortalTemeContext context, ICourseMapper courseMapper) {
            _context = context;
            this.courseMapper = courseMapper;
        }

        // GET: api/StudyDomains
        [HttpGet]
        [Authorize(Policy = AuthorizationConstants.CanViewStudyDomainsPolicy)]
        public async Task<ActionResult<IEnumerable<StudyDomainDTO>>> GetStudyDomains() {
            return (await _context.StudyDomains
                .ToListAsync())
                .Select(domain => courseMapper.MapStudyDomain(domain))
                .ToList();
        }

        // GET: api/StudyDomains/5
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationConstants.CanViewStudyDomainsPolicy)]
        public async Task<ActionResult<StudyDomainDTO>> GetStudyDomain(Guid id) {
            var studyDomain = await _context.StudyDomains.FindAsync(id);

            if (studyDomain == null) {
                return NotFound();
            }

            return courseMapper.MapStudyDomain(studyDomain);
        }

        // PUT: api/StudyDomains/5
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationConstants.CanEditStudyDomainsPolicy)]
        public async Task<IActionResult> PutStudyDomain(Guid id, StudyDomainDTO studyDomain) {
            if (id != studyDomain.Id) {
                return BadRequest();
            }

            var domain = courseMapper.MapStudyDomainDTO(studyDomain);

            _context.Entry(domain).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!StudyDomainExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StudyDomains
        [HttpPost]
        [Authorize(Policy = AuthorizationConstants.CanEditStudyDomainsPolicy)]
        public async Task<ActionResult<StudyDomainDTO>> PostStudyDomain(StudyDomainDTO studyDomain) {

            var domain = courseMapper.MapStudyDomainDTO(studyDomain);

            _context.StudyDomains.Add(domain);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudyDomain",
                new { id = domain.Id },
                courseMapper.MapStudyDomain(domain)
            );
        }

        // DELETE: api/StudyDomains/5
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationConstants.CanEditStudyDomainsPolicy)]
        public async Task<ActionResult<StudyDomainDTO>> DeleteStudyDomain(Guid id) {
            var studyDomain = await _context.StudyDomains.FindAsync(id);
            if (studyDomain == null) {
                return NotFound();
            }

            _context.StudyDomains.Remove(studyDomain);
            await _context.SaveChangesAsync();

            return courseMapper.MapStudyDomain(studyDomain);
        }

        private bool StudyDomainExists(Guid id) {
            return _context.StudyDomains.Any(e => e.Id == id);
        }
    }
}
