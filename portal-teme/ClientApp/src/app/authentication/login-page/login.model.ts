import { ModelErrors } from "../../http.models";

export interface LoginModel {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface LoginResponse {
    status: AuthorizationStatus;
    errors: ModelErrors;
}

export enum AuthorizationStatus {
    Success,
    TwoFactorRequired,
    LockedOut,
    InvalidCredentials
}