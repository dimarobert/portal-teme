export interface RegisterModel {
    email: string;
    password: string;
    confirmPassword: string;
}

export interface RegisterResponse {
    status: RegisterStatus;
    errors: string[];
}

export interface RegisterError {
    [key: string]: string[];
}

export enum RegisterStatus {
    Error
}