using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portalteme.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("[action]")]
        [Authorize]
        public IEnumerable<object> Hello() {
            return User.Claims.Select(c => new { c.Type, c.Value });
        }
    }
}
