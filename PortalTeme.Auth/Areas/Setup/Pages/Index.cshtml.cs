using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalTeme.Auth.Services;
using PortalTeme.Common.Application;
using PortalTeme.Common.Authorization;
using PortalTeme.Data.Application;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Pages.Setup {
    public class IndexModel : PageModel {
        private readonly UserManager<User> userManager;
        private readonly IdentityContext identityContext;
        private readonly AppInitialization appInitialization;

        public IndexModel(UserManager<User> userManager, IdentityContext identityContext, AppInitialization appInitialization) {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.appInitialization = appInitialization;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet() {

        }

        public async Task<IActionResult> OnPostAsync() {

            if (!ModelState.IsValid)
                return Page();

            var adminUser = new User {
                UserName = Input.AdminEmail,
                Email = Input.AdminEmail,
                FirstName = Input.FirstName ?? "",
                LastName = Input.LastName ?? ""
            };

            var result = await userManager.CreateAsync(adminUser);
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            result = await userManager.AddToRoleAsync(adminUser, AuthorizationConstants.AdministratorRoleName);
            if (!result.Succeeded) {
                await userManager.DeleteAsync(adminUser);

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            identityContext.ApplicationSettings.Add(new ApplicationSetting { Name = ApplicationConstants.InitializedSettingName });
            await identityContext.SaveChangesAsync();
            appInitialization.Reset();

            return LocalRedirect("/");
        }
    }

    public class InputModel {

        [Required, EmailAddress, Display(Name = "Email")]
        public string AdminEmail { get; set; }

        [Required, Display(Name = "Password")]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "The two passwords do not match."), Display(Name = "Password Confirmation")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}