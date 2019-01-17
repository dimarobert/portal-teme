import { EditModel } from './base.model';
import { CourseRef } from './course.model';

export interface Assignment extends AssignmentEdit {
    id: string;

    slug: string;
}

export interface AssignmentEdit extends EditModel {
    course: CourseRef;

    name: string;
    description: string;

    startDate: Date;
    endDate: Date;

    dateAdded?: Date;
    lastUpdated?: Date;
}

export interface AssignmentEntry extends AssignmentEntryEdit {
    id: string;
}

export interface AssignmentEntryEdit extends EditModel {
    courseId: string;

    assignmentId: string;

    studentId: string;

    state: AssignmentEntryState;

    grading?: number;
}

export enum AssignmentEntryState {
    Submitted,
    Reviewed,
    Graded
}

