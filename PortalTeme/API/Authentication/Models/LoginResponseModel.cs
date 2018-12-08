using System.Collections.Generic;

namespace PortalTeme.API.Authentication.Models {
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
