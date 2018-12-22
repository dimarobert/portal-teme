using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PortalTeme.Auth.Areas.Identity.Managers;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity {

    public class ProfileService : ProfileService<User> {
        private readonly ApplicationUserManager userManager;
        private readonly ILogger<ProfileService> logger;

        public ProfileService(ApplicationUserManager userManager, 
            IUserClaimsPrincipalFactory<User> claimsFactory,
            ILogger<ProfileService> logger): base(userManager, claimsFactory) {
            this.userManager = userManager;
            this.logger = logger;
        }

        public override async Task IsActiveAsync(IsActiveContext context) {
            await base.IsActiveAsync(context);
            logger.LogDebug("IsActive called from: {caller}", context.Caller);

            if (!context.IsActive)
                return;

            var user = await userManager.GetUserAsync(context.Subject);
            context.IsActive = !await userManager.IsLockedOutAsync(user);

            logger.LogDebug("IsActive evaluated to: {isActive}", context.IsActive);
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            await base.GetProfileDataAsync(context);

            //context.AddRequestedClaims(await GetProfileClaims(context.Subject));
            context.LogIssuedClaims(logger);
        }

        // Profile Claims are: name, family_name, given_name, middle_name, nickname, preferred_username,
        //                     profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.
        //private async Task<IEnumerable<Claim>> GetProfileClaims(ClaimsPrincipal subject) {
        //    var user = await userManager.GetUserAsync(subject);
        //    if (user is null)
        //        return Enumerable.Empty<Claim>();

        //    return new List<Claim> {
        //        new Claim("email", await userManager.GetEmailAsync(user)),
        //        new Claim("given_name", await userManager.GetFirstNameAsync(user)),
        //        new Claim("family_name", await userManager.GetLastNameAsync(user))
        //    };
        //}
    }
}
