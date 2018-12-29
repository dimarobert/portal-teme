using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Data.Managers {
    public class AppUserManager : IUserManager {
        private readonly PortalTemeContext context;

        public AppUserManager(PortalTemeContext context) {
            this.context = context;
        }

        public async Task<User> GetUserAsync(ClaimsPrincipal principal) {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return await context.Users.FirstAsync(u => u.Id == userId);
        }
    }
}
