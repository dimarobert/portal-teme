import { EditModel, BaseModel } from './base.model';
import { CourseRef, User } from './course.model';

export interface Assignment extends AssignmentEdit {
    id: string;

    slug: string;

    assignmentVariants: AssignmentVariant[];
}

export interface AssignmentEdit extends EditModel {
    course: CourseRef;

    name: string;
    description: string;

    type: AssignmentType;

    numberOfDuplicates: number;

    startDate: Date;
    endDate: Date;

    dateAdded?: Date;
    lastUpdated?: Date;
}

export enum AssignmentType {
    SingleHomework,
    SingleChoiceList,
    MultipleChoiceList,
    CustomAssignedHomework
}

export interface AssignmentVariant extends BaseModel {
    assignmentId: string;

    name: string;
    description: string;

    studentId?: string;
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

