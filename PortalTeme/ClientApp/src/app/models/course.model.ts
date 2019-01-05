import { BaseModel, NamedModel } from './base.model';

export interface Course extends CourseEdit {

    assistants: User[];

    groups: StudyGroupRef[];
    students: User[];
}

export interface CourseEdit extends BaseModel {
    courseDef: CourseDefinitionRef;
    professor: User;
}

export interface CourseDefinitionRef extends NamedModel { }

export interface User extends BaseModel {
    firstName: string;
    lastName: string;
}

export interface StudyGroupRef {
    groupId: string;
    name: string;
}