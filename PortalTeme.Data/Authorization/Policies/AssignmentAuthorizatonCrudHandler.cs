using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using PortalTeme.Data.Managers;
using PortalTeme.Data.Models;
using System.Threading.Tasks;

namespace PortalTeme.Data.Authorization.Policies {
    public class AssignmentAuthorizatonCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, Assignment> {
        private CourseAuthorizatonCrudHandler courseHandler;

        public AssignmentAuthorizatonCrudHandler(IUserManager userManager, PortalTemeContext temeContext) {
            courseHandler = new CourseAuthorizatonCrudHandler(userManager, temeContext);
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Assignment resource) {
            return courseHandler.HandleCourseRequirementAsync(context, requirement, resource.Course);
        }
    }
}
