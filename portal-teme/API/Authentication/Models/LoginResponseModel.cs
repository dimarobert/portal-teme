using System.Collections.Generic;

namespace portal_teme.API.Authentication.Models {
    public class LoginResponseModel {

        public AuthorizationStatus Status { get; set; }

        public Dictionary<string, IEnumerable<string>> Errors { get; set; }

    }

    public enum AuthorizationStatus {
        Success,
        TwoFactorRequired,
        LockedOut,
        InvalidCredentials
    }
}
