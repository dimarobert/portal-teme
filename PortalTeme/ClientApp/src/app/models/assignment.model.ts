import { EditModel, BaseModel } from './base.model';
import { CourseRef, User } from './course.model';

export interface UserAssignment extends Assignment {
    assignedTask: AssignmentTask;
}

export interface Assignment extends AssignmentEdit {
    id: string;

    slug: string;

    tasks: AssignmentTask[];
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

export interface AssignmentTask extends AssignmentTaskEdit {
    id: string;

    studentsAssigned: User[];
}

export interface AssignmentTaskEdit extends EditModel {
    assignmentId: string;

    name: string;
    description: string;

    studentsAssigned?: User[];
}

export interface StudentAssignedTask extends BaseModel {

    task: AssignmentTask;

    studentId: string;
    student: User;

    state: StudentAssignedTaskState;

    grading?: number;

    submissions: TaskSubmission[];
}

export enum StudentAssignedTaskState {
    Assigned,
    Submitted,
    Reviewed,
    Graded
}

export interface TaskSubmission extends EditModel {
    studentTaskId: string;
    dateAdded: Date;
    files: TaskSubmissionFile[];
}

export interface TaskSubmissionFile extends EditModel {
    name: string;
    description: string;
    fileType: TaskSubmissionFileType;
}

export enum TaskSubmissionFileType {
    SourceCode,
    Project,
    Essay
}