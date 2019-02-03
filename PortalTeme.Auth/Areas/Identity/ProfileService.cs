using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
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
        private readonly IUserClaimsPrincipalFactory<User> claimsFactory;

        public ProfileService(ApplicationUserManager userManager,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            ILogger<ProfileService> logger) : base(userManager, claimsFactory, logger) {
            this.userManager = userManager;
            this.claimsFactory = claimsFactory;
        }

        public override async Task IsActiveAsync(IsActiveContext context) {
            await base.IsActiveAsync(context);
            Logger?.LogDebug("IsActive called from: {caller}", context.Caller);

            if (!context.IsActive)
                return;

            var user = await userManager.GetUserAsync(context.Subject);
            context.IsActive = !await userManager.IsLockedOutAsync(user);

            Logger?.LogDebug("IsActive evaluated to: {isActive}", context.IsActive);
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await UserManager.FindByIdAsync(sub);
            if (user is null) {
                Logger?.LogWarning("No user found matching subject Id: {0}", sub);
                return;
            }

            var principal = await claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();

            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));

            context.AddRequestedClaims(claims);

            if (!(Logger is null))
                context.LogIssuedClaims(Logger);
        }

        // Profile Claims are: name, family_name, given_name, middle_name, nickname, preferred_username,
        //                     profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.
    }
}
