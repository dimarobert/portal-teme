using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Common.Authorization;
using PortalTeme.Data.Identity;
using PortalTeme.Data.Models.Assignments.Projections;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Data.Authorization.Policies {
    public class AssignmentEntriesAuthorizatonCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, AssignmentEntryProjection> {
        private readonly UserManager<User> userManager;
        private readonly PortalTemeContext temeContext;
        private readonly CourseAuthorizatonCrudHandler courseHandler;

        public AssignmentEntriesAuthorizatonCrudHandler(UserManager<User> userManager, PortalTemeContext temeContext) {
            courseHandler = new CourseAuthorizatonCrudHandler(userManager, temeContext);
            this.userManager = userManager;
            this.temeContext = temeContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, AssignmentEntryProjection resource) {

            if (!context.PendingRequirements.Any())
                return;

            if (requirement.Name == Operations.ViewAssignmentEntries.Name) {
                var currentUser = await userManager.GetUserAsync(context.User);

                if (currentUser.Id == resource.StudentId) {
                    context.Succeed(requirement);
                    return;
                }

                var isProfessorOrAssistant = await temeContext.Courses
                    .Where(c => c.Id == resource.CourseId)
                    .Where(c => c.ProfessorId == currentUser.Id || c.Assistants.Any(assistant => assistant.AssistantId == currentUser.Id))
                    .AnyAsync();

                if (isProfessorOrAssistant) {
                    context.Succeed(requirement);
                    return;
                }
            }

        }
    }
}
