using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Authentication.Models {
    public class RegisterModel {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
