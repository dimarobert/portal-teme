using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin {
    public class ViewUsersInRoleModel : PageModel {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ViewUsersInRoleModel(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public List<UserDTO> Users { get; set; } = new List<UserDTO>();

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public async Task<IActionResult> OnGetAsync(string roleId) {
            if (string.IsNullOrWhiteSpace(roleId))
                return RedirectToPage("UserRoles");

            var role = await GetRoleAsync(roleId);
            if (role is null)
                return Page();

            Input.RoleId = roleId;
            Input.RoleName = role.Name;

            Users = (await userManager.GetUsersInRoleAsync(role.Name)).Select(u => new UserDTO {
                Id = u.Id,
                Email = u.Email
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveUserAsync(string userId) {

            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrWhiteSpace(userId)) {
                ModelState.AddModelError(string.Empty, "No userId specified.");
                return Page();
            }

            var role = await GetRoleAsync(Input.RoleId);
            if (role is null)
                return Page();

            var user = await userManager.FindByIdAsync(userId);
            if (user is null) {
                ModelState.AddModelError(string.Empty, "Could not find a user with the specified userId.");
                return Page();
            }

            if (!await userManager.IsInRoleAsync(user, role.Name))
                return RedirectToPage("ViewUsersInRole", new { roleId = Input.RoleId });

            var result = await userManager.RemoveFromRoleAsync(user, role.Name);
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage("ViewUsersInRole", new { roleId = Input.RoleId });
        }

        private async Task<IdentityRole> GetRoleAsync(string roleId) {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                ModelState.AddModelError(string.Empty, "Could not find any role with the specified Id.");

            return role;
        }

        public class InputModel {

            public string RoleId { get; set; }

            public string RoleName { get; set; }

        }

        public class UserDTO {
            public string Id { get; set; }

            public string Email { get; set; }
        }
    }
}