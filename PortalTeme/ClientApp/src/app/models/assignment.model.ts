import { NamedModel } from './base.model';
import { CourseEdit } from './course.model';

export interface AssignmentEdit extends NamedModel {

    course: CourseEdit;

    description: string;

    startDate: Date;
    endDate: Date;

    dateAdded?: Date;
    lastUpdated?: Date;
}

export interface Assignment extends AssignmentEdit {
    dateAdded: Date;
    lastUpdated: Date;
}