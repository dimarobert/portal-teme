import { BaseModel, EditModel } from './base.model';
import { Assignment } from './assignment.model';

export interface Course extends BaseModel {
    courseDef: CourseDefinitionRef;
    professor: User;

    assistants: User[];

    groups: CourseGroup[];
    students: User[];

    assignments: Assignment[];
}

export interface CourseEdit extends EditModel {
    courseDef: CourseDefinitionRef;
    professor: UserRef;
}

export interface CourseRef extends EditModel { }

export interface CourseDefinitionRef extends BaseModel {
    name?: string;
    slug?: string;
}

export interface UserRef extends BaseModel { }

export interface User extends UserRef {
    firstName: string;
    lastName: string;
}

export interface CourseGroup extends CourseRelation {
    groupId: string;
    name: string;
}

export interface CourseAssistant extends CourseRelation {
    assistant: User;
}

export interface CourseStudent extends CourseRelation {
    student: User;
}

export interface CourseRelation {
    courseId: string;
}