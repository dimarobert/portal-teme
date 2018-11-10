using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using portal_teme.API.Authentication.Models;
using portal_teme.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portal_teme.API.Authentication.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager) {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel) {

            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded) {
                return Ok(new LoginResponseModel {
                    Status = AuthorizationStatus.Success
                });
            }

            if (result.RequiresTwoFactor) {
                return Ok(new LoginResponseModel {
                    Status = AuthorizationStatus.TwoFactorRequired
                });
            }

            if (result.IsLockedOut) {
                return this.Unauthorized(new LoginResponseModel {
                    Status = AuthorizationStatus.LockedOut
                });
            }

            return this.Unauthorized(new LoginResponseModel {
                Status = AuthorizationStatus.InvalidCredentials,
                Message = "Invalid login attempt"
            });

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel) {

            var user = new User {
                UserName = registerModel.Email,
                Email = registerModel.Email
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded) {

                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            return BadRequest(new RegisterResponseModel {
                Status = RegisterStatus.Error,
                Errors = result.Errors.Select(err => err.Description).ToList()
            });
        }

    }
}