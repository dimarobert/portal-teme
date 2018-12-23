using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Authorization;
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
    public class AssignmentEntriesController : ControllerBase {
        private readonly PortalTemeContext _context;

        public AssignmentEntriesController(PortalTemeContext context) {
            _context = context;
        }

        // GET: api/AssignmentEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentEntry>>> GetAssignmentEntries() {
            return await _context.AssignmentEntries.ToListAsync();
        }

        // GET: api/AssignmentEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentEntry>> GetAssignmentEntry(Guid id) {
            var assignmentEntry = await _context.AssignmentEntries.FindAsync(id);

            if (assignmentEntry is null)
                return NotFound();

            return assignmentEntry;
        }

        // PUT: api/AssignmentEntries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignmentEntry(Guid id, AssignmentEntry assignmentEntry) {
            if (id != assignmentEntry.Id)
                return BadRequest();

            _context.Entry(assignmentEntry).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!AssignmentEntryExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/AssignmentEntries
        [HttpPost]
        public async Task<ActionResult<AssignmentEntry>> PostAssignmentEntry(AssignmentEntry assignmentEntry) {
            _context.AssignmentEntries.Add(assignmentEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssignmentEntry", new { id = assignmentEntry.Id }, assignmentEntry);
        }

        // DELETE: api/AssignmentEntries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AssignmentEntry>> DeleteAssignmentEntry(Guid id) {
            var assignmentEntry = await _context.AssignmentEntries.FindAsync(id);
            if (assignmentEntry is null)
                return NotFound();

            _context.AssignmentEntries.Remove(assignmentEntry);
            await _context.SaveChangesAsync();

            return assignmentEntry;
        }

        private bool AssignmentEntryExists(Guid id) {
            return _context.AssignmentEntries.Any(e => e.Id == id);
        }
    }
}
