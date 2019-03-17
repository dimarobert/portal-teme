using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models;
using PortalTeme.API.Models.Courses;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
using PortalTeme.Data.Identity;
using PortalTeme.Helpers;
using PortalTeme.Services;
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
        private readonly ICacheService cache;
        private readonly IAuthorizationService authorizationService;
        private readonly ICourseMapper courseMapper;
        private readonly UserManager<User> userManager;

        public CoursesController(PortalTemeContext context, ICacheService cache, IAuthorizationService authorizationService, ICourseMapper courseMapper, UserManager<User> userManager) {
            _context = context;
            this.cache = cache;
            this.authorizationService = authorizationService;
            this.courseMapper = courseMapper;
            this.userManager = userManager;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseViewDTO>>> GetCourses() {
            var courses = await _context.Courses
                .Include(c => c.CourseInfo)
                .Include(c => c.Professor)
                .Include(c => c.Assistants).ThenInclude(c => c.Assistant)
                .Include(c => c.Groups).ThenInclude(c => c.Group)
                .Include(c => c.Students).ThenInclude(s => s.Student).ThenInclude(si => si.User)
                .Include(c => c.Assignments)
                .ToListAsync();

            var results = new List<CourseViewDTO>();
            foreach (var course in courses) {
                if ((await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanViewCoursePolicy)).Succeeded)
                    results.Add(courseMapper.MapCourseView(course));
            }

            return results;
        }

        // GET: api/Courses/Ref
        [HttpGet("Ref")]
        public async Task<ActionResult<IEnumerable<CourseEditDTO>>> GetCoursesRef() {

            // TODO: Caching by user, which can be cleared entirely.
            //var cachedCourses = await cache.GetCoursesRefAsync();
            //if (!(cachedCourses is null))
            //    return cachedCourses;

            var courses = await _context.Courses
                .Include(c => c.CourseInfo)
                .Include(c => c.Professor)
                .ToListAsync();

            var results = new List<CourseEditDTO>();
            foreach (var course in courses) {
                if ((await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanViewCoursePolicy)).Succeeded)
                    results.Add(courseMapper.MapCourseEdit(course));
            }

            //await cache.SetCoursesRefAsync(results);

            return results;
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseViewDTO>> GetCourse(Guid id) {
            var course = await _context.Courses
                .Include(c => c.CourseInfo)
                .Include(c => c.Professor)
                .Include(c => c.Assistants).ThenInclude(c => c.Assistant)
                .Include(c => c.Groups).ThenInclude(c => c.Group)
                .Include(c => c.Students).ThenInclude(s => s.Student).ThenInclude(si => si.User)
                //.Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            return courseMapper.MapCourseView(course);
        }

        // GET: api/Courses/Slug/my-course
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<CourseViewDTO>> GetCourseBySlug(string slug) {

            var cachedCourse = await cache.GetCourseBySlugAsync(slug);
            if (!(cachedCourse is null))
                return cachedCourse;

            var course = await _context.Courses
                .Include(c => c.CourseInfo)
                .Include(c => c.Professor)
                .Include(c => c.Assistants).ThenInclude(c => c.Assistant)
                .Include(c => c.Groups).ThenInclude(c => c.Group)
                .Include(c => c.Students).ThenInclude(s => s.Student).ThenInclude(si => si.User)
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.CourseInfo.Slug == slug);

            if (course is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanViewCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            var courseDto = courseMapper.MapCourseView(course);

            await cache.SetCourseBySlugAsync(courseDto);

            return courseDto;
        }


        // GET: api/Courses/5/members
        [HttpGet("{id}/members")]
        public async Task<ActionResult<List<UserDTO>>> GetCourseMembers(Guid id) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cachedMembers = await cache.GetCourseMembersAsync(id);
            if (!(cachedMembers is null))
                return cachedMembers;

            var course = await _context.Courses.FindAsync(id);

            if (course is null)
                return NotFound();

            var courseGroups = await _context.Courses
                .Where(c => c.Id == id)
                .Select(c => c.Groups.Select(g => g.Group))
                .FirstOrDefaultAsync();

            var members = new List<UserDTO>();
            foreach (var group in courseGroups) {
                var users = await userManager.GetUsersForClaimAsync(new System.Security.Claims.Claim("study_group", group.Code));
                members.AddRange(users.Select(user => courseMapper.MapUser(user)));
            }

            var nonGroupMembers = await _context.Courses
                .Where(c => c.Id == id)
                .Select(c => c.Students.Select(s => s.Student.User))
                .FirstOrDefaultAsync();

            members.AddRange(nonGroupMembers.Select(user => courseMapper.MapUser(user)));

            members = members.Distinct(ProjectionEqualityComparer<UserDTO>.Create(u => u.Id))
                .OrderBy(u => u.FirstName)
                .ToList();

            await cache.SetCourseMembersAsync(id, members);

            return members;
        }


        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(Guid id, CourseEditDTO course) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != course.Id)
                return BadRequest("The request id parameter did not match the course id in the request body.");

            var dbCourse = courseMapper.MapCourseEditDTO(course);

            var authorization = await authorizationService.AuthorizeAsync(User, dbCourse, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Entry(dbCourse).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();

                await cache.ClearCourseCacheAsync(id, dbCourse.CourseInfo.Slug);

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
        public async Task<ActionResult<CourseEditDTO>> PostCourse(CourseEditDTO course) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dbCourse = courseMapper.MapCourseEditDTO(course);

            var authorization = await authorizationService.AuthorizeAsync(User, dbCourse, AuthorizationConstants.CanCreateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Courses.Add(dbCourse);
            await _context.SaveChangesAsync();

            dbCourse = await _context.Courses
                .Include(c => c.CourseInfo)
                .Include(c => c.Professor)
                .FirstOrDefaultAsync(c => c.Id == dbCourse.Id);

            await cache.ClearCoursesRefCacheAsync();

            return CreatedAtAction("GetCourse", new { id = dbCourse.Id }, courseMapper.MapCourseEdit(dbCourse));
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseViewDTO>> DeleteCourse(Guid id) {
            var course = await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.CourseInfo)
                .Include(c => c.Assistants)
                .Include(c => c.Groups)
                .Include(c => c.Students)
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanDeleteCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            await cache.ClearCourseCacheAsync(id, course.CourseInfo.Slug);

            return courseMapper.MapCourseView(course);
        }


        // POST: api/Courses/5/AddAssistant
        [HttpPost("{courseId}/AddAssistant")]
        public async Task<ActionResult<CourseAssistantDTO>> PostCourseAssistant(Guid courseId, CourseAssistantDTO assistant) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (courseId != assistant.CourseId) {
                ModelState.AddModelError(string.Empty, "The courseId request parameter did not match the courseId property in the data.");
                return BadRequest(ModelState);
            }

            var cAssistant = courseMapper.MapCourseAssistantDTO(assistant);

            var existingAssistant = await _context.CourseAssistants.FindAsync(cAssistant.CourseId, cAssistant.AssistantId);
            if (existingAssistant != null) {
                ModelState.AddModelError(string.Empty, "The assistent you are trying to add already exists.");
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.CourseInfo)
                .FirstOrDefaultAsync(c => c.Id == cAssistant.CourseId);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanEditCourseAssistantsPolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.CourseAssistants.Add(cAssistant);
            await _context.SaveChangesAsync();

            cAssistant = await _context.CourseAssistants
                .Include(ca => ca.Assistant)
                .FirstOrDefaultAsync(ca => ca.CourseId == cAssistant.CourseId && ca.AssistantId == cAssistant.AssistantId);

            await cache.ClearCourseCacheAsync(courseId, course.CourseInfo.Slug);

            return CreatedAtAction("GetCourse", new { id = cAssistant.CourseId }, courseMapper.MapCourseAssistant(cAssistant));
        }

        // POST: api/Courses/5/DeleteAssistant/6
        [HttpDelete("{courseId}/DeleteAssistant/{assistantId}")]
        public async Task<ActionResult<CourseAssistantDTO>> DeleteCourseAssistant(Guid courseId, string assistantId) {
            var courseAssistant = await _context.CourseAssistants
                .Include(ca => ca.Course).ThenInclude(c => c.CourseInfo)
                .Include(ca => ca.Assistant)
                .FirstOrDefaultAsync(ca => ca.CourseId == courseId && ca.AssistantId == assistantId);

            if (courseAssistant is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, courseAssistant.Course, AuthorizationConstants.CanEditCourseAssistantsPolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.CourseAssistants.Remove(courseAssistant);
            await _context.SaveChangesAsync();

            await cache.ClearCourseCacheAsync(courseId, courseAssistant.Course.CourseInfo.Slug);

            return courseMapper.MapCourseAssistant(courseAssistant);
        }

        // POST: api/Courses/5/AddGroup
        [HttpPost("{courseId}/AddGroup")]
        public async Task<ActionResult<CourseGroupDTO>> PostCourseGroup(Guid courseId, CourseGroupDTO group) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (courseId != group.CourseId) {
                ModelState.AddModelError(string.Empty, "The courseId request parameter did not match the courseId property in the data.");
                return BadRequest(ModelState);
            }

            var cGroup = courseMapper.MapCourseGroupDTO(group);

            var existingGroup = await _context.CourseGroups.FindAsync(cGroup.CourseId, cGroup.GroupId);
            if (existingGroup != null) {
                ModelState.AddModelError(string.Empty, "The group you are trying to add already exists.");
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
               .Include(c => c.Professor)
               .Include(c => c.CourseInfo)
               .FirstOrDefaultAsync(c => c.Id == cGroup.CourseId);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.CourseGroups.Add(cGroup);
            await _context.SaveChangesAsync();

            cGroup = await _context.CourseGroups
                .Include(cg => cg.Group)
                .FirstOrDefaultAsync(cg => cg.CourseId == cGroup.CourseId && cg.GroupId == cGroup.GroupId);

            await cache.ClearCourseCacheAsync(courseId, course.CourseInfo.Slug);

            return CreatedAtAction("GetCourse", new { id = cGroup.CourseId }, courseMapper.MapCourseGroup(cGroup));
        }

        // POST: api/Courses/5/DeleteGroup/6
        [HttpDelete("{courseId}/DeleteGroup/{groupId}")]
        public async Task<ActionResult<CourseGroupDTO>> DeleteCourseGroup(Guid courseId, Guid groupId) {
            var courseGroup = await _context.CourseGroups
                .Include(cg => cg.Course).ThenInclude(c => c.CourseInfo)
                .Include(cg => cg.Group)
                .FirstOrDefaultAsync(cg => cg.CourseId == courseId && cg.GroupId == groupId);

            if (courseGroup is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, courseGroup.Course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.CourseGroups.Remove(courseGroup);
            await _context.SaveChangesAsync();

            await cache.ClearCourseCacheAsync(courseId, courseGroup.Course.CourseInfo.Slug);

            return courseMapper.MapCourseGroup(courseGroup);
        }

        // POST: api/Courses/5/AddStudent
        [HttpPost("{courseId}/AddStudent")]
        public async Task<ActionResult<CourseStudentDTO>> PostCourseStudent(Guid courseId, CourseStudentDTO student) {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (courseId != student.CourseId) {
                ModelState.AddModelError(string.Empty, "The courseId request parameter did not match the courseId property in the data.");
                return BadRequest(ModelState);
            }

            var cStudent = courseMapper.MapCourseStudentDTO(student);

            var existingStudent = await _context.CourseStudents.FindAsync(cStudent.CourseId, cStudent.StudentId);
            if (existingStudent != null) {
                ModelState.AddModelError(string.Empty, "The student you are trying to add already exists.");
                return BadRequest(ModelState);
            }

            var course = await _context.Courses
               .Include(c => c.Professor)
               .Include(c => c.CourseInfo)
               .FirstOrDefaultAsync(c => c.Id == cStudent.CourseId);

            var authorization = await authorizationService.AuthorizeAsync(User, course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.CourseStudents.Add(cStudent);
            await _context.SaveChangesAsync();

            cStudent = await _context.CourseStudents
                .Include(cs => cs.Student).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(cs => cs.CourseId == cStudent.CourseId && cs.StudentId == cStudent.StudentId);

            await cache.ClearCourseCacheAsync(courseId, course.CourseInfo.Slug);

            return CreatedAtAction("GetCourse", new { id = cStudent.CourseId }, courseMapper.MapCourseStudent(cStudent));
        }

        // POST: api/Courses/5/DeleteStudent/6
        [HttpDelete("{courseId}/DeleteStudent/{studentId}")]
        public async Task<ActionResult<CourseStudentDTO>> DeleteCourseStudent(Guid courseId, string studentId) {
            var courseStudent = await _context.CourseStudents
                .Include(cs => cs.Course).ThenInclude(c => c.CourseInfo)
                .Include(cs => cs.Student).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId);

            if (courseStudent is null)
                return NotFound();

            var authorization = await authorizationService.AuthorizeAsync(User, courseStudent.Course, AuthorizationConstants.CanUpdateCoursePolicy);
            if (!authorization.Succeeded)
                return Forbid();

            _context.CourseStudents.Remove(courseStudent);
            await _context.SaveChangesAsync();

            await cache.ClearCourseCacheAsync(courseId, courseStudent.Course.CourseInfo.Slug);

            return courseMapper.MapCourseStudent(courseStudent);
        }

        private bool CourseExists(Guid id) {
            return _context.Courses.Any(e => e.Id == id);
        }

    }
}
