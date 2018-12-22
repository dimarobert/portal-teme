export interface ApplicationSettings {
    user?: UserSettings;
}

export interface UserSettings {
    email: string;
    firstName: string;
    lastName: string;
}