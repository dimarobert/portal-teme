import { ModelErrors } from "../../http.models";

export interface RegisterModel {
    email: string;
    password: string;
    confirmPassword: string;
}

export interface RegisterResponse {
    status: RegisterStatus;
    errors: ModelErrors;
}

export enum RegisterStatus {
    Error
}