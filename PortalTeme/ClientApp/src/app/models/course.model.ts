import { BaseModel, NamedModel } from './base.model';

export interface Course extends CourseEdit {

    assistants: Assistant[];

    groups: StudyGroupRef[];
    students: StudentRef[];
}

export interface CourseEdit extends BaseModel {
    courseDef: CourseDefinitionRef;
    professor: Professor;
}

export interface CourseDefinitionRef extends NamedModel { }

export interface Professor extends BaseModel {
    firstName: string;
    lastName: string;
}

export interface Assistant extends BaseModel {
    firstName: string;
    lastName: string;
}

export interface StudentRef extends BaseModel {
    firstName: string;
    lastName: string;
}

export interface StudyGroupRef {
    groupId: string;
    name: string;
}