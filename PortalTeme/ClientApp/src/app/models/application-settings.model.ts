export interface ApplicationSettings {
    user: UserSettings;
}

export interface UserSettings {
    authToken: string;
    email: string;
    firstName: string;
    lastName: string;
}