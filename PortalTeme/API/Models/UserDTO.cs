using System.ComponentModel.DataAnnotations;

namespace PortalTeme.API.Models {
    public class UserDTO {

        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

}
