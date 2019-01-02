import { BaseModel } from './base.model';

export interface CourseDefinition extends BaseModel {
    year: string;
    semester: Semester,
    name: string;
}

export enum Semester {
    First = 0,
    Second = 1
}