using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using PortalTeme.Common.Authorization;
using PortalTeme.Data.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Data.Authorization.Policies {
    public class AdminAuthorizatonHandler : AuthorizationHandler<OperationAuthorizationRequirement> {

        public AdminAuthorizatonHandler(UserManager<User> userManager, PortalTemeContext temeContext) { }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement) {

            if (!context.PendingRequirements.Contains(requirement))
                return Task.CompletedTask;

            var isAdmin = context.User.IsInRole(AuthorizationConstants.AdministratorRoleName);
            if (isAdmin)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
