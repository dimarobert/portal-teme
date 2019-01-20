using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models.Assignments;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
using PortalTeme.Data.Models;
using PortalTeme.Data.Models.Assignments.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthorizationConstants.AdministratorPolicy)]
    public class AssignmentEntriesController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly IAssignmentMapper assignmentMapper;
        private readonly IAuthorizationService authorizationService;

        public AssignmentEntriesController(PortalTemeContext context, IAssignmentMapper assignmentMapper, IAuthorizationService authorizationService) {
            _context = context;
            this.assignmentMapper = assignmentMapper;
            this.authorizationService = authorizationService;
        }

        //// GET: api/AssignmentEntries/ForAssignment/5
        //[HttpGet("ForAssignment/{assignmentId}")]
        //public async Task<ActionResult<IEnumerable<AssignmentEntryDTO>>> GetAssignmentEntries(Guid assignmentId) {
        //    var entries = await _context.AssignmentEntries
        //        .Where(ae => ae.AssignedTask.AssignmentId == assignmentId)
        //        .Select(ae => new AssignmentEntryProjection {
        //            Id = ae.Id,
        //            AssignmentTaskId = ae.AssignmentTask.Id,
        //            CourseId = ae.AssignmentTask.Assignment.Course.Id,
        //            StudentId = ae.Student.UserId,
        //            State = ae.State,
        //            Grading = ae.Grading,
        //            Versions = ae.Versions
        //        })
        //        .ToListAsync();

        //    var results = new List<AssignmentEntryDTO>();
        //    foreach (var entry in entries) {
        //        var authorization = await authorizationService.AuthorizeAsync(User, entry, AuthorizationConstants.CanViewAssignmentEntriesPolicy);
        //        if (!authorization.Succeeded)
        //            results.Add(assignmentMapper.MapAssignmentEntryProjection(entry));
        //    }

        //    return results;
        //}

        //// GET: api/AssignmentEntries/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AssignmentEntryDTO>> GetAssignmentEntry(Guid id) {
        //    var assignmentEntry = await _context.AssignmentEntries
        //        .Where(ae => ae.Id == id)
        //        .Select(ae => new AssignmentEntryProjection {
        //            Id = ae.Id,
        //            AssignmentTaskId = ae.AssignedTask.Id,
        //            CourseId = ae.AssignedTask.Assignment.Course.Id,
        //            StudentId = ae.Student.UserId,
        //            State = ae.State,
        //            Grading = ae.Grading,
        //            Versions = ae.Versions
        //        })
        //        .FirstOrDefaultAsync();

        //    if (assignmentEntry is null)
        //        return NotFound();

        //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanViewAssignmentEntriesPolicy);
        //    if (!authorization.Succeeded)
        //        return Forbid();

        //    return assignmentMapper.MapAssignmentEntryProjection(assignmentEntry);
        //}

        //// PUT: api/AssignmentEntries/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAssignmentEntry(Guid id, AssignmentEntryDTO assignmentEntryDto) {
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (id != assignmentEntryDto.Id)
        //        return BadRequest();

        //    var assignmentEntry = assignmentMapper.MapAssignmentEntryProjectionDTO(assignmentEntryDto);

        //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
        //    if (!authorization.Succeeded)
        //        return Forbid();

        //    _context.Entry(assignmentEntry).State = EntityState.Modified;

        //    try {
        //        await _context.SaveChangesAsync();
        //    } catch (DbUpdateConcurrencyException) {
        //        if (!AssignmentEntryExists(id))
        //            return NotFound();
        //        else
        //            throw;
        //    }

        //    return NoContent();
        //}

        //// POST: api/AssignmentEntries
        //[HttpPost]
        //public async Task<ActionResult<AssignmentEntryDTO>> PostAssignmentEntry(AssignmentEntryDTO assignmentEntryDto) {
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var assignmentEntry = assignmentMapper.MapAssignmentEntryProjectionDTO(assignmentEntryDto);

        //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
        //    if (!authorization.Succeeded)
        //        return Forbid();

        //    _context.AssignmentEntries.Add(assignmentEntry);
        //    await _context.SaveChangesAsync();

        //    var entry = await _context.AssignmentEntries
        //        .Where(ae => ae.Id == assignmentEntry.Id)
        //        .Select(ae => new AssignmentEntryProjection {
        //            Id = ae.Id,
        //            AssignmentTaskId = ae.AssignedTask.Id,
        //            CourseId = ae.AssignedTask.Assignment.Course.Id,
        //            StudentId = ae.Student.UserId,
        //            State = ae.State,
        //            Grading = ae.Grading,
        //            Versions = ae.Versions
        //        })
        //        .FirstOrDefaultAsync();

        //    return CreatedAtAction("GetAssignmentEntry", new { id = entry.Id }, assignmentMapper.MapAssignmentEntryProjection(entry));
        //}

        //// DELETE: api/AssignmentEntries/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<AssignmentEntryDTO>> DeleteAssignmentEntry(Guid id) {
        //    var assignmentEntry = await _context.AssignmentEntries.FindAsync(id);
        //    if (assignmentEntry is null)
        //        return NotFound();

        //    var authorization = await authorizationService.AuthorizeAsync(User, assignmentEntry, AuthorizationConstants.CanEditAssignmentEntriesPolicy);
        //    if (!authorization.Succeeded)
        //        return Forbid();

        //    var projection = await _context.AssignmentEntries
        //          .Where(ae => ae.Id == assignmentEntry.Id)
        //          .Select(ae => new AssignmentEntryProjection {
        //              Id = ae.Id,
        //              AssignmentTaskId = ae.AssignedTask.Id,
        //              CourseId = ae.AssignedTask.Assignment.Course.Id,
        //              StudentId = ae.Student.UserId,
        //              State = ae.State,
        //              Grading = ae.Grading,
        //              Versions = ae.Versions
        //          })
        //          .FirstOrDefaultAsync();

        //    _context.AssignmentEntries.Remove(assignmentEntry);
        //    await _context.SaveChangesAsync();

        //    return assignmentMapper.MapAssignmentEntryProjection(projection);
        //}

        //private bool AssignmentEntryExists(Guid id) {
        //    return _context.AssignmentEntries.Any(e => e.Id == id);
        //}
    }
}
