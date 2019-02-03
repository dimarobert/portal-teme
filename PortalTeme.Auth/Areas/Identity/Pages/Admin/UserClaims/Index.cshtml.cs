using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin.UserClaims {
    public class IndexModel : PageModel {
        private readonly UserManager<User> userManager;

        public List<ClaimDTO> Claims { get; set; } = new List<ClaimDTO>();

        public string UserId { get; set; }

        public IndexModel(UserManager<User> userManager) {
            this.userManager = userManager;
        }

        public async Task OnGetAsync(string userId) {
            UserId = userId;
            var user = await userManager.FindByIdAsync(userId);

            Claims = (await userManager.GetClaimsAsync(user))
                .Select(claim => new ClaimDTO {
                    Name = claim.Type,
                    Value = claim.Value
                }).ToList();
        }

        public async Task<IActionResult> OnPostDeleteClaimAsync(string userId, string claimName, string claimValue) {

            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim(claimName, claimValue));
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage("Index", new { userId });
        }
    }

    public class ClaimDTO {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}