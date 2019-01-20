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
    public class StudentTasksAuthorizatonCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, StudentTaskProjection> {
        private readonly UserManager<User> userManager;
        private readonly PortalTemeContext temeContext;
        private readonly CourseAuthorizatonCrudHandler courseHandler;

        public StudentTasksAuthorizatonCrudHandler(UserManager<User> userManager, PortalTemeContext temeContext) {
            courseHandler = new CourseAuthorizatonCrudHandler(userManager, temeContext);
            this.userManager = userManager;
            this.temeContext = temeContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, StudentTaskProjection resource) {

            if (!context.PendingRequirements.Any())
                return;

            if (requirement.Name == Operations.ViewStudentTask.Name) {
                var currentUser = await userManager.GetUserAsync(context.User);

                if (currentUser.Id == resource.StudentId) {
                    context.Succeed(requirement);
                    return;
                }

                if (await IsCourseProfessorOrAssistant(currentUser, resource)) {
                    context.Succeed(requirement);
                    return;
                }
            } else if (requirement.Name == Operations.EditStudentTask.Name) {
                var currentUser = await userManager.GetUserAsync(context.User);
                if (await IsCourseProfessorOrAssistant(currentUser, resource)) {
                    context.Succeed(requirement);
                    return;
                }
            }
        }

        private async Task<bool> IsCourseProfessorOrAssistant(User currentUser, StudentTaskProjection resource) {
            var isProfessorOrAssistant = await temeContext.Courses
                .Where(c => c.Id == resource.CourseId)
                .Where(c => c.ProfessorId == currentUser.Id || c.Assistants.Any(assistant => assistant.AssistantId == currentUser.Id))
                .AnyAsync();

            return isProfessorOrAssistant;
        }
    }
}
