using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalTeme.API.Mappers;
using PortalTeme.API.Models;
using PortalTeme.Common.Authorization;
using PortalTeme.Data;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {

        private readonly UserManager<User> userManager;
        private ICourseMapper courseMapper;

        public UsersController(UserManager<User> userManager, ICourseMapper courseMapper) {
            this.userManager = userManager;
            this.courseMapper = courseMapper;
        }

        // GET: api/Users/Professors
        [HttpGet("Professors")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetProfessors() {
            return (await GetUsersInRole(AuthorizationConstants.ProfessorRoleName)).ToList();
        }

        // GET: api/Users/Assistants
        [HttpGet("Assistants")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAssistants() {
            return (await GetUsersInRole(AuthorizationConstants.AssistantRoleName)).ToList();
        }

        // GET: api/Users/Students
        [HttpGet("Students")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetStudents() {
            return (await GetUsersInRole(AuthorizationConstants.StudentRoleName)).ToList();
        }

        private async Task<IEnumerable<UserDTO>> GetUsersInRole(string role) {
            return (await userManager.GetUsersInRoleAsync(role))
                .Select(user => courseMapper.MapUser(user));
        }
    }
}