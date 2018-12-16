using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalTeme.Auth.Areas.Identity.Managers;
using PortalTeme.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Pages.Account.Manage {
    public partial class IndexModel : PageModel {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            ApplicationUserManager userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var firstName = await _userManager.GetFirstNameAsync(user);
            var lastName = await _userManager.GetLastNameAsync(user);

            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel {
                FirstName = firstName,
                LastName = lastName,

                Email = email,
                PhoneNumber = phoneNumber
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var claimsToUpdate = new List<Claim>();

            var firstName = await _userManager.GetFirstNameAsync(user);
            if (Input.FirstName != firstName) {
                var setFirstNameResult = await _userManager.SetFirstNameAsync(user, Input.FirstName);
                if (!setFirstNameResult.Succeeded) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting first name for user with ID '{userId}'.");
                }
            }
            if (Input.FirstName != User.FindFirst(Common.UserProfile.UserProfileConstants.GivenNameClaim)?.Value) {
                claimsToUpdate.Add(new Claim(Common.UserProfile.UserProfileConstants.GivenNameClaim, Input.FirstName));
            }

            var lastName = await _userManager.GetLastNameAsync(user);
            if (Input.LastName != lastName) {
                var setLastNameResult = await _userManager.SetLastNameAsync(user, Input.LastName);
                if (!setLastNameResult.Succeeded) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting lst name for user with ID '{userId}'.");
                }
            }
            if (Input.LastName != User.FindFirst(Common.UserProfile.UserProfileConstants.FamilyNameClaim)?.Value) {
                claimsToUpdate.Add(new Claim(Common.UserProfile.UserProfileConstants.FamilyNameClaim, Input.LastName));
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email) {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber) {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (claimsToUpdate.Any()) {
                await _userManager.RemoveClaimsAsync(user, claimsToUpdate);
                await _userManager.AddClaimsAsync(user, claimsToUpdate);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
