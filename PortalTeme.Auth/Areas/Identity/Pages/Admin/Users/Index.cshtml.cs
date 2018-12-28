using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Admin.Users {
    public class IndexModel : PageModel {
        private readonly UserManager<User> userManager;

        public IndexModel(UserManager<User> userManager) {
            this.userManager = userManager;
        }

        public List<UserDTO> Users { get; set; } = new List<UserDTO>();

        public async Task OnGetAsync() {
            Users = await userManager.Users.OrderBy(u => u.Email).Select(u => new UserDTO {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId) {

            if (!ModelState.IsValid)
                return Page();

            var user = await userManager.FindByIdAsync(userId);
            if (user is null) {
                ModelState.AddModelError(string.Empty, "Could not find the user.");
                return Page();
            }

            var currentUser = await userManager.GetUserAsync(User);

            if (user.Id == currentUser.Id) {
                ModelState.AddModelError(string.Empty, "The current user cannot be deleted.");
                return Page();
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            return RedirectToPage("Index");
        }

    }

    public class UserDTO {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}