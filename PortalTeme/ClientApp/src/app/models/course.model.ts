import { BaseModel, EditModel } from './base.model';

export interface Course extends BaseModel {
    courseDef: CourseDefinitionRef;
    professor: UserRef;

    assistants: User[];

    groups: StudyGroupRef[];
    students: User[];
}

export interface CourseEdit extends EditModel {
    courseDef: CourseDefinitionRef;
    professor: UserRef;
}

export interface CourseDefinitionRef extends BaseModel {
    name?: string;
}

export interface UserRef extends BaseModel { }

export interface User extends UserRef {
    firstName: string;
    lastName: string;
}

export interface StudyGroupRef {
    groupId: string;
    name: string;
}