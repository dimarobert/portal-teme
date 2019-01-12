import { EditModel } from './base.model';
import { CourseRef } from './course.model';

export interface Assignment extends AssignmentEdit {
    id: string;
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