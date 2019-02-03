using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin.UserClaims {
    public class AddClaimModel : PageModel {
        private readonly UserManager<User> userManager;

        public string UserId { get; set; }

        [BindProperty]
        public AddClaimInput Input { get; set; }

        public AddClaimModel(UserManager<User> userManager) {
            this.userManager = userManager;
        }

        public void OnGet(string userId) {
            UserId = userId;
        }

        public async Task<IActionResult> OnPostAsync(string userId) {

            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(Input.Name, Input.Value));
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage("Index", new { userId });
        }
    }

    public class AddClaimInput {

        [Required, Display(Name = "Claim Name")]
        public string Name { get; set; }


        [Required, Display(Name = "Claim Value")]
        public string Value { get; set; }
    }
}