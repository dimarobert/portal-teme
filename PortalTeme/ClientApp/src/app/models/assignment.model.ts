import { EditModel, BaseModel } from './base.model';
import { CourseRef } from './course.model';

export interface UserAssignment extends Assignment {
    assignedTask: AssignmentVariant;
}

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
    SingleTask,
    SingleChoiceList,
    MultipleChoiceList,
    CustomAssignedTasks
}

const AssignmentTypeText = {
    SingleTask: "Single task",
    SingleTaskText: "A single exercise/task will be defined for the entire class to solve.",

    SingleChoiceList: "List of tasks - single choice",
    SingleChoiceListText: "A list of exercises/tasks will be defined, each student will have to chose his own task. A task can only be chosen by a single student.",

    MultipleChoiceList: "List of tasks - multiple choice",
    MultipleChoiceListText: "A list of exercises/tasks will be defined, each student will have to chose his own task. A task can be chosen by the specified number of students.",

    CustomAssignedTasks: "Individually assigned tasks",
    CustomAssignedTasksText: "Each task will be manually assigned to each student."
}

export default AssignmentTypeText;

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

