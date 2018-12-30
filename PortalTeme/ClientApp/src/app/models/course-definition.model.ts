export interface CourseDefinition {
    id: string;
    year: string;
    semester: Semester,
    name: string;
}

export enum Semester {
    First = 0,
    Second = 1
}