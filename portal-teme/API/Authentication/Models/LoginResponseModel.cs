namespace portal_teme.API.Authentication.Models {
    public class LoginResponseModel {

        public AuthorizationStatus Status { get; set; }

        public string Message { get; set; }

    }

    public enum AuthorizationStatus {
        Success,
        TwoFactorRequired,
        LockedOut,
        InvalidCredentials
    }
}
