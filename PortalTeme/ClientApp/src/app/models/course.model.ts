import { NamedModel } from './base.model';

export interface Course extends NamedModel {
    professor: string;
    assistants: Assistant[];

    groups: StudyGroupRef[];
    students: StudentRef[];
}

export interface Professor {
    professorId: string;
    firstName: string;
    lastName: string;
}

export interface Assistant {
    assistantId: string;
    firstName: string;
    lastName: string;
}

export interface StudentRef {
    studentId: string;
    firstName: string;
    lastName: string;
}

export interface StudyGroupRef {
    groupId: string;
    name: string;
}