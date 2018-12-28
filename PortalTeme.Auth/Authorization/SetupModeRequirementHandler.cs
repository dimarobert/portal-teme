using Microsoft.AspNetCore.Authorization;
using PortalTeme.Auth.Services;
using System;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Authorization {
    public class SetupModeRequirementHandler : AuthorizationHandler<SetupModeRequirement> {
        private readonly AppInitialization init;

        public SetupModeRequirementHandler(AppInitialization init) {
            this.init = init;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SetupModeRequirement requirement) {
            if (!await init.IsInitialized())
                context.Succeed(requirement);
        }
    }
}
