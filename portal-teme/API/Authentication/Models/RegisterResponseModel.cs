using System.Collections.Generic;

namespace portal_teme.API.Authentication.Models {
    internal class RegisterResponseModel {
        public RegisterStatus Status { get; set; }
        public List<string> Errors { get; set; }
    }

    public enum RegisterStatus {
        Error
    }
}