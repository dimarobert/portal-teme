export interface ApplicationSettings {
    user?: UserSettings;
}

export interface UserSettings {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
}