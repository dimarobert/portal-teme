using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin {
    public class AddRoleModel : PageModel {

        private readonly RoleManager<IdentityRole> roleManager;

        public AddRoleModel(RoleManager<IdentityRole> roleManager) {
            this.roleManager = roleManager;
        }

        [BindProperty]
        public RoleDTO Input { get; set; }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPost() {
            if (!ModelState.IsValid)
                return Page();

            var result = await roleManager.CreateAsync(new IdentityRole {
                Name = Input.Name
            });
            
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            return RedirectToPage("UserRoles");
        }
    }
}