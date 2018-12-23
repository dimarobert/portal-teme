using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin {
    public class UserRolesModel : PageModel {
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRolesModel(RoleManager<IdentityRole> roleManager) {
            this.roleManager = roleManager;
        }

        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();

        public void OnGet() {

            Roles = roleManager.Roles.Select(role => new RoleDTO {
                Id = role.Id,
                Name = role.Name
            }).ToList();

        }

        public async Task<IActionResult> OnPostDeleteRoleAsync(string roleId) {

            if (roleId is null)
                return ReturnError(new[] { "No Role Id provided." });

            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                return ReturnError(new[] { "Could not find the role with the provided Id." });

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return ReturnError(result.Errors.Select(e => e.Description));

            return RedirectToPage();
        }

        private IActionResult ReturnError(IEnumerable<string> errors) {
            foreach (var error in errors) {
                ModelState.AddModelError(string.Empty, error);
            }
            OnGet();
            return Page();
        }
    }

    public class RoleDTO {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}