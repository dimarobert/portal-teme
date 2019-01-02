using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using PortalTeme.Common.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Data.Authorization.Policies {
    public class GroupsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement> {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement) {

            switch (requirement.Name) {
                case nameof(Operations.ViewGroups):
                    var canView = context.User.IsInRole(AuthorizationConstants.AdministratorRoleName) ||
                    context.User.IsInRole(AuthorizationConstants.ProfessorRoleName);
                    if (canView)
                        context.Succeed(requirement);
                    break;

                case nameof(Operations.EditGroups):
                    var isAdmin = context.User.IsInRole(AuthorizationConstants.AdministratorRoleName);
                    if (isAdmin)
                        context.Succeed(requirement);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
