export interface LoginModel {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface LoginResponse {
    status: AuthorizationStatus;
    message: string;
}

export enum AuthorizationStatus {
    Success,
    TwoFactorRequired,
    LockedOut,
    InvalidCredentials
}