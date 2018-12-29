using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Data.Managers {
    public interface IUserManager {

        Task<User> GetUserAsync(ClaimsPrincipal user);

    }
}
