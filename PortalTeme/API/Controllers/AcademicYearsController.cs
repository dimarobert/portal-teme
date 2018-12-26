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
    public class AcademicYearsController : ControllerBase {
        private readonly PortalTemeContext _context;

        public AcademicYearsController(PortalTemeContext context) {
            _context = context;
        }

        // GET: api/AcademicYears
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicYear>>> GetAcademicYears() {
            return await _context.AcademicYears.ToListAsync();
        }

        // GET: api/AcademicYears/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicYear>> GetAcademicYear(Guid id) {
            var academicYear = await _context.AcademicYears.FindAsync(id);

            if (academicYear is null)
                return NotFound();

            return academicYear;
        }

        // PUT: api/AcademicYears/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcademicYear(Guid id, AcademicYear academicYear) {
            if (id != academicYear.Id)
                return BadRequest();

            _context.Entry(academicYear).State = EntityState.Modified;

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
        public async Task<ActionResult<AcademicYear>> PostAcademicYear(AcademicYear academicYear) {
            _context.AcademicYears.Add(academicYear);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcademicYear", new { id = academicYear.Id }, academicYear);
        }

        // DELETE: api/AcademicYears/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AcademicYear>> DeleteAcademicYear(Guid id) {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear is null)
                return NotFound();

            _context.AcademicYears.Remove(academicYear);
            await _context.SaveChangesAsync();

            return academicYear;
        }

        private bool AcademicYearExists(Guid id) {
            return _context.AcademicYears.Any(e => e.Id == id);
        }
    }
}
