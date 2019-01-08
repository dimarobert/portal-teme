using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Common.Authorization;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Data.Authorization.Policies {
    public class CourseAuthorizatonCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, Course> {
        private readonly UserManager<User> userManager;
        private readonly PortalTemeContext temeContext;

        public CourseAuthorizatonCrudHandler(UserManager<User> userManager, PortalTemeContext temeContext) {
            this.userManager = userManager;
            this.temeContext = temeContext;
        }

        public async Task HandleCourseRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Course resource) {

            if (requirement.Name == Operations.Create.Name) {
                var canCreate = context.User.IsInRole(AuthorizationConstants.ProfessorRoleName);
                if (canCreate) {
                    context.Succeed(requirement);
                    return;
                }
            }

            var currentUser = await userManager.GetUserAsync(context.User);

            var isOwner = currentUser.Id == resource.ProfessorId;
            if (isOwner) {
                context.Succeed(requirement);
                return;
            }

            if (requirement.Name == Operations.Update.Name || requirement.Name == Operations.Read.Name) {
                var isAssignedAssistant = resource.Assistants.Any(assistant => assistant.AssistantId == currentUser.Id);
                if (isAssignedAssistant) {
                    context.Succeed(requirement);
                    return;
                }
            }

            if (requirement.Name == Operations.Read.Name) {

                var isAssignedStudent = resource.Students.Any(student => student.StudentId == currentUser.Id);
                if (!isAssignedStudent && resource.Groups.Any()) {
                    var student = await temeContext.Students
                        .Include(s => s.Group)
                        .FirstOrDefaultAsync(s => s.UserId == currentUser.Id);
                    if (!(student is null))
                        isAssignedStudent = resource.Groups.Any(group => group.GroupId == student.Group.Id);
                }

                if (isAssignedStudent) {
                    context.Succeed(requirement);
                    return;
                }
            }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Course resource) {
            await HandleCourseRequirementAsync(context, requirement, resource);
        }
    }
}
