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
    public class GroupsController : ControllerBase {
        private readonly PortalTemeContext _context;
        private readonly ICourseMapper courseMapper;

        public GroupsController(PortalTemeContext context, ICourseMapper courseMapper) {
            _context = context;
            this.courseMapper = courseMapper;
        }

        // GET: api/Groups
        [HttpGet]
        [Authorize(Policy = AuthorizationConstants.CanViewGroupsPolicy)]
        public async Task<ActionResult<IEnumerable<GroupDTO>>> GetGroups() {
            return (await _context.Groups
                .Include(g => g.Domain)
                .Include(g => g.Year)
                .ToListAsync())
                .Select(group => courseMapper.MapGroup(group))
                .ToList();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationConstants.CanViewGroupsPolicy)]
        public async Task<ActionResult<GroupDTO>> GetGroup(Guid id) {
            var group = await _context.Groups.FindAsync(id);

            if (group == null) {
                return NotFound();
            }

            return courseMapper.MapGroup(group);
        }

        // PUT: api/Groups/5
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationConstants.CanEditGroupsPolicy)]
        public async Task<IActionResult> PutGroup(Guid id, GroupDTO group) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != group.Id) {
                return BadRequest();
            }

            var domain = await _context.StudyDomains.FirstOrDefaultAsync(sd => sd.Id == group.Domain);
            if (domain is null) {
                ModelState.AddModelError("domain", "Invalid domain id.");
                return BadRequest(ModelState);
            }

            var year = await _context.AcademicYears.FirstOrDefaultAsync(y => y.Id == group.Year);
            if (year is null) {
                ModelState.AddModelError("year", "Invalid year id.");
                return BadRequest(ModelState);
            }

            var dbGroup = courseMapper.MapGroupDTO(group, domain, year);

            _context.Entry(dbGroup).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!GroupExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Groups
        [HttpPost]
        [Authorize(Policy = AuthorizationConstants.CanEditGroupsPolicy)]
        public async Task<ActionResult<GroupDTO>> PostGroup(GroupDTO group) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var domain = await _context.StudyDomains.FirstOrDefaultAsync(sd => sd.Id == group.Domain);
            if (domain is null)
                ModelState.AddModelError("domain", "Invalid domain id.");

            var year = await _context.AcademicYears.FirstOrDefaultAsync(y => y.Id == group.Year);
            if (year is null)
                ModelState.AddModelError("year", "Invalid year id.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dbGroup = courseMapper.MapGroupDTO(group, domain, year);

            _context.Groups.Add(dbGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup",
                new { id = dbGroup.Id },
                courseMapper.MapGroup(dbGroup)
            );
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationConstants.CanEditGroupsPolicy)]
        public async Task<ActionResult<GroupDTO>> DeleteGroup(Guid id) {
            var group = await _context.Groups
                .Include(g => g.Domain)
                .Include(g => g.Year)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (group == null) {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return courseMapper.MapGroup(group);
        }

        private bool GroupExists(Guid id) {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
