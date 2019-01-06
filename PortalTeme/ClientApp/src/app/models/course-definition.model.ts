import { BaseModel, NamedModel } from './base.model';

export interface CourseDefinition extends NamedModel {
    year: string;
    semester: Semester;
}

export enum Semester {
    First = 0,
    Second = 1
}