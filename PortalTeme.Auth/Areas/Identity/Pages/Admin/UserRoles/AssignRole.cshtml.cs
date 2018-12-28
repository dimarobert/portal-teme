using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin {
    public class AssignRoleModel : PageModel {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AssignRoleModel(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public List<UserDTO> Users { get; set; } = new List<UserDTO>();
        public string RoleId { get; set; }

        [BindProperty]
        public AssignRoleDTO Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleId) {

            if (string.IsNullOrWhiteSpace(roleId))
                return RedirectToPage("ViewUsersInRole");

            RoleId = roleId;
            Users = await userManager.Users.OrderBy(u => u.Email).Select(u => new UserDTO {
                Id = u.Id,
                Email = u.Email
            }).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleId) {
            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrWhiteSpace(roleId)) {
                ModelState.AddModelError(string.Empty, "Invalid roleId route parameter.");
                return Page();
            }

            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null) {
                ModelState.AddModelError(string.Empty, "Could not find any role with the specified Id.");
                return Page();
            }

            var user = await userManager.FindByIdAsync(Input.UserId);
            if (string.IsNullOrWhiteSpace(roleId)) {
                ModelState.AddModelError("Input.UserId", "Could not find any user with that User Id.");
                return Page();
            }

            if (!await userManager.IsInRoleAsync(user, role.Name)) {
                var result = await userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded) {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            return RedirectToPage("ViewUsersInRole", new { roleId });
        }
    }

    public class UserDTO {
        public string Id { get; set; }

        public string Email { get; set; }
    }

    public class AssignRoleDTO {

        [Required, Display(Name = "User Email")]
        public string UserId { get; set; }
    }
}