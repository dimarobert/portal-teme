using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models;
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
    public class AcademicYearsController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly ICourseMapper courseMapper;

        public AcademicYearsController(PortalTemeContext context, ICourseMapper courseMapper) {
            _context = context;
            this.courseMapper = courseMapper;
        }

        // GET: api/AcademicYears
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicYearDTO>>> GetAcademicYears() {
            return (await _context.AcademicYears.ToListAsync())
                .Select(y => courseMapper.MapYear(y))
                .ToList();
        }

        // GET: api/AcademicYears/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicYearDTO>> GetAcademicYear(Guid id) {
            var academicYear = await _context.AcademicYears.FindAsync(id);

            if (academicYear is null)
                return NotFound();

            return courseMapper.MapYear(academicYear);
        }

        // PUT: api/AcademicYears/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcademicYear(Guid id, AcademicYearDTO academicYear) {
            if (id != academicYear.Id)
                return BadRequest();

            var year = courseMapper.MapYearDTO(academicYear);

            _context.Entry(year).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!AcademicYearExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/AcademicYears
        [HttpPost]
        public async Task<ActionResult<AcademicYearDTO>> PostAcademicYear(AcademicYearDTO academicYear) {
            var year = courseMapper.MapYearDTO(academicYear);

            _context.AcademicYears.Add(year);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcademicYear", new { id = year.Id }, year);
        }

        // DELETE: api/AcademicYears/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AcademicYearDTO>> DeleteAcademicYear(Guid id) {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear is null)
                return NotFound();

            _context.AcademicYears.Remove(academicYear);
            await _context.SaveChangesAsync();

            return courseMapper.MapYear(academicYear);
        }

        private bool AcademicYearExists(Guid id) {
            return _context.AcademicYears.Any(e => e.Id == id);
        }
    }
}
